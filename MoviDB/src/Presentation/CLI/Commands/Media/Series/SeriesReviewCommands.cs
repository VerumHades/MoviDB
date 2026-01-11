using MoviDB.Application.DTOs;
using MoviDB.Application.Services;
using MoviDB.Presentation.CLI.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MoviDB.Presentation.CLI;

public static class SeriesReviewCommands
{
    public static ICommand AddSeriesReviewCommand(SeriesReviewService service) =>
        new SimpleCommand<string>(
            name: "AddSeriesReview",
            description: "Adds a review to a series using the series title and username.",
            parameters: new List<CommandParameter>
            {
                new("mediaTitle", "Title of the series", typeof(string), false),
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
                output.WriteLine($"Review added to series '{creationData.MediaTitle}' by user '{creationData.Username}'.");
                return "Review added.";
            }
        );

    public static ICommand UpdateSeriesReviewCommand(SeriesReviewService service) =>
        new SimpleCommand<string>(
            name: "UpdateSeriesReview",
            description: "Updates an existing review for a series using the series title and username.",
            parameters: new List<CommandParameter>
            {
                new("reviewId", "ID of the review to update", typeof(int), false),
                new("mediaTitle", "Title of the series", typeof(string), false),
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
                output.WriteLine($"Review {reviewId} updated for series '{updateData.MediaTitle}'.");
                return "Review updated.";
            }
        );

    public static ICommand RemoveSeriesReviewCommand(SeriesReviewService service) =>
        new SimpleCommand<string>(
            name: "RemoveSeriesReview",
            description: "Removes a review from a series using username and series title.",
            parameters: new List<CommandParameter>
            {
                new("username", "Username of the reviewer", typeof(string), false),
                new("mediaTitle", "Title of the series", typeof(string), false)
            },
            action: async (parameters, output) =>
            {
                string username = Convert.ToString(parameters["username"])!;
                string mediaTitle = Convert.ToString(parameters["mediaTitle"])!;

                await service.RemoveReviewAsync(username, mediaTitle);
                output.WriteLine($"Review by '{username}' removed for series '{mediaTitle}'.");
                return "Review removed.";
            }
        );
}
