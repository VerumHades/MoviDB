set nocount on;
set xact_abort on;

begin try
begin transaction;

    -- ==========================
    -- drop triggers
    -- ==========================
drop trigger if exists trg_review_after_insert;
drop trigger if exists trg_review_after_update;
drop trigger if exists trg_review_after_delete;

-- ==========================
-- drop views
-- ==========================
drop view if exists vw_movie;
drop view if exists vw_series;

-- ==========================
-- drop tables (reverse dependency order)
-- ==========================
drop table if exists review;
drop table if exists library_entry;
drop table if exists episode;
drop table if exists season;
drop table if exists series;
drop table if exists movie;
drop table if exists media;
drop table if exists genre;
drop table if exists [user];

-- ==========================
-- create tables
-- ==========================

create table [user] (
                        id int identity(1,1) primary key,
    username nvarchar(255) not null unique,
    password_hash nvarchar(255) not null,
    role nvarchar(20) not null check (role in ('normal', 'moderator')),
    created_at datetime not null default getdate()
    );

create table genre (
                       id int identity(1,1) primary key,
                       name nvarchar(255) not null unique
);

create table media (
                       id int identity(1,1) primary key,
                       created_at datetime not null default getdate(),
                       title nvarchar(255) not null,
                       description nvarchar(300) null,
                       type nvarchar(20) not null check (type in ('movie', 'series')),
                       rating_count int not null default 0 check (rating_count >= 0),
                       rating_sum float not null default 0 check (rating_sum >= 0)
);

create table movie (
                       media_id int primary key references media(id) on delete cascade,
                       genre_id int not null references genre(id),
                       duration_minutes int null check (duration_minutes > 0)
);

create table series (
                        media_id int primary key references media(id) on delete cascade,
                        genre_id int not null references genre(id)
);

create table season (
                        id int identity(1,1) primary key,
                        series_id int not null references series(media_id) on delete cascade,
                        title nvarchar(255) not null,
                        number int not null check (number > 0),
                        constraint uq_season_perseries unique (series_id, number)
);

create table episode (
                         id int identity(1,1) primary key,
                         season_id int not null references season(id) on delete cascade,
                         title nvarchar(255) not null,
                         episode_number int not null check (episode_number > 0),
                         created_at datetime not null default getdate(),
                         constraint uq_episode_perseason unique (season_id, episode_number)
);

create table library_entry (
                               media_id int not null references media(id),
                               user_id int not null references [user](id),
                               watched bit not null default 0,
                               created_at datetime not null default getdate(),
                               constraint pk_library_entry primary key (media_id, user_id)
);

create table review (
                        id int identity(1,1) primary key,
                        media_id int not null references media(id),
                        user_id int not null references [user](id),
                        title nvarchar(255) not null,
                        content nvarchar(max) not null,
                        rating float not null check (rating >= 0 and rating <= 5),
                        created_at datetime not null default getdate(),
                        constraint uq_review_peruser unique (media_id, user_id)
);

-- ==========================
-- create views
-- ==========================

create view vw_movie as
select
    m.id as id,
    m.title,
    m.description,
    g.id as genre_id,
    g.name as genre_name,
    m.rating_count,
    m.rating_sum,
    case
        when m.rating_count > 0 then m.rating_sum / m.rating_count
        else 0
        end as rating,
    mv.duration_minutes,
    m.created_at
from media m
         inner join movie mv on mv.media_id = m.id
         inner join genre g on g.id = mv.genre_id
where m.type = 'movie';

create view vw_series as
select
    s.media_id as id,
    m.title,
    m.description,
    g.name as genre_name,
    count(se.id) as season_count,
    m.rating_count,
    m.rating_sum,
    case
        when m.rating_count > 0 then m.rating_sum / m.rating_count
        else 0
        end as rating,
    m.created_at
from series s
         inner join media m on s.media_id = m.id
         inner join genre g on s.genre_id = g.id
         left join season se on se.series_id = s.media_id
group by
    s.media_id,
    m.title,
    m.description,
    g.name,
    m.rating_count,
    m.rating_sum,
    m.created_at;

-- ==========================
-- create triggers
-- ==========================

create trigger trg_review_after_insert
    on review
    after insert
    as
begin
        set nocount on;

update m
set
    m.rating_count = m.rating_count + 1,
    m.rating_sum = m.rating_sum + i.rating
    from media m
            inner join inserted i on m.id = i.media_id;
end;

create trigger trg_review_after_update
    on review
    after update
              as
begin
        set nocount on;

update m
set m.rating_sum = m.rating_sum - d.rating + i.rating
    from media m
            inner join inserted i on m.id = i.media_id
    inner join deleted d on d.id = i.id
where i.rating <> d.rating;
end;

create trigger trg_review_after_delete
    on review
    after delete
as
begin
        set nocount on;

update m
set
    m.rating_count = m.rating_count - 1,
    m.rating_sum = m.rating_sum - d.rating
    from media m
            inner join deleted d on m.id = d.media_id;
end;

commit transaction;
end try
begin catch
if @@trancount > 0
        rollback transaction;

    throw;
end catch;
