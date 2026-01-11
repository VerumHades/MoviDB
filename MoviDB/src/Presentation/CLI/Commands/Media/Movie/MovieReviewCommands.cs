namespace MoviDB.Presentation.CLI.Commands;

using MoviDB.Application.DTOs;
using MoviDB.Application.Services;
using MoviDB.Presentation.CLI.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public static class MovieReviewCommands
{
    public static ICommand AddMovieReviewCommand(MovieReviewService service) =>
        new SimpleCommand<string>(
            name: "AddMovieReview",
            description: "Adds a review to a movie using the movie title and username.",
            parameters: new List<CommandParameter>
            {
                new("mediaTitle", "Title of the movie", typeof(string), false),
                new("username", "Username of the reviewer", typeof(string), false),
                new("title", "Title of the review", typeof(string), false),
                new("content", "Content of the review", typeof(string), false),
                new("rating", "Rating value", typeof(double), false)
            },
            action: async (parameters, output) =>
            {
                var creationData = new ReviewCreationData(
                    MediaTitle: Convert.ToString(parameters["mediaTitle"])!,
                    Username: Convert.ToString(parameters["username"])!,
                    Title: Convert.ToString(parameters["title"])!,
                    Content: Convert.ToString(parameters["content"])!,
                    Rating: Convert.ToDouble(parameters["rating"])
                );

                await service.AddReviewAsync(creationData);
                output.WriteLine($"Review added to movie '{creationData.MediaTitle}' by user '{creationData.Username}'.");
                return $"Review added.";
            }
        );

    public static ICommand UpdateMovieReviewCommand(MovieReviewService service) =>
        new SimpleCommand<string>(
            name: "UpdateMovieReview",
            description: "Updates an existing review for a movie using the movie title and username.",
            parameters: new List<CommandParameter>
            {
                new("reviewId", "ID of the review to update", typeof(int), false),
                new("mediaTitle", "Title of the movie", typeof(string), false),
                new("username", "Username of the reviewer", typeof(string), false),
                new("title", "Updated title of the review", typeof(string), false),
                new("content", "Updated content", typeof(string), false),
                new("rating", "Updated rating", typeof(double), false)
            },
            action: async (parameters, output) =>
            {
                var updateData = new ReviewUpdateData(
                    MediaTitle: Convert.ToString(parameters["mediaTitle"])!,
                    Username: Convert.ToString(parameters["username"])!,
                    Title: Convert.ToString(parameters["title"])!,
                    Content: Convert.ToString(parameters["content"])!,
                    Rating: Convert.ToDouble(parameters["rating"])
                );

                int reviewId = Convert.ToInt32(parameters["reviewId"]);

                await service.UpdateReviewAsync(reviewId, updateData);
                output.WriteLine($"Review {reviewId} updated for movie '{updateData.MediaTitle}'.");
                return $"Review updated.";
            }
        );

    public static ICommand RemoveMovieReviewCommand(MovieReviewService service) =>
        new SimpleCommand<string>(
            name: "RemoveMovieReview",
            description: "Removes a review from a movie using the username and movie title.",
            parameters: new List<CommandParameter>
            {
                new("username", "Username of the reviewer", typeof(string), false),
                new("mediaTitle", "Title of the movie", typeof(string), false)
            },
            action: async (parameters, output) =>
            {
                string username = Convert.ToString(parameters["username"])!;
                string mediaTitle = Convert.ToString(parameters["mediaTitle"])!;

                await service.RemoveReviewAsync(username, mediaTitle);

                output.WriteLine($"Review by '{username}' removed for movie '{mediaTitle}'.");
                return $"Review removed.";
            }
        );
}
