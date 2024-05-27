using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.API.Tests.Mock;

public class MockData
{
    public static readonly IEnumerable<Content?> MockGetManyContentsResponse = new List<Content?>()
    {
        new Content(
            id: Guid.NewGuid(),
            title: "MC1_Title",
            subTitle: "MC1_SubTitle",
            description: "MC1_Description",
            imageUrl: "MC1_Url",
            duration: 60,
            startTime: DateTime.UtcNow,
            endTime: DateTime.UtcNow.AddHours(1),
            genreList: new List<string> { "Genre1", "Genre2" }),

        new Content(
            id: Guid.NewGuid(),
            title: "MC2_Title",
            subTitle: "MC2_SubTitle",
            description: "MC2_Description",
            imageUrl: "MC2_Url",
            duration: 90,
            startTime: DateTime.UtcNow,
            endTime: DateTime.UtcNow.AddHours(1.5),
            genreList: new List<string> { "Genre2", "Genre3" }),

        new Content(
            id: Guid.NewGuid(),
            title: "MC3_Title",
            subTitle: "MC3_SubTitle",
            description: "MC3_Description",
            imageUrl: "MC3_Url",
            duration: 120,
            startTime: DateTime.UtcNow,
            endTime: DateTime.UtcNow.AddHours(2),
            genreList: new List<string> { "Genre3", "Genre4" })
    };


    public static Content MockGetContentResponse(Guid id)
    {
        return new Content(
            id: id,
            title: "MC_Title",
            subTitle: "MC_SubTitle",
            description: "MC_Description",
            imageUrl: "MC_Url",
            duration: 60,
            startTime: DateTime.UtcNow,
            endTime: DateTime.UtcNow.AddHours(1),
            genreList: new List<string> { "Genre1", "Genre2", "Genre3" });
    }


    public static readonly ContentInput MockCreateContentRequest = new ContentInput
    {
        Title = "MC_Title",
        SubTitle = "MC_SubTitle",
        Description = "MC_Description",
        ImageUrl = "MC_Url",
        Duration = 60,
        StartTime = DateTime.UtcNow,
        EndTime = DateTime.UtcNow.AddHours(1),
    };

    public static readonly Content MockCreateContentResponse = new Content(
        id: Guid.NewGuid(),
        title: "MC_Title",
        subTitle: "MC_SubTitle",
        description: "MC_Description",
        imageUrl: "MC_Url",
        duration: 60,
        startTime: DateTime.UtcNow,
        endTime: DateTime.UtcNow.AddHours(1),
        genreList: new List<string> { "Genre1", "Genre2", "Genre3" });


    public static Content MockUpdateContentOriginal(Guid id)
    {
        return new Content(
            id: id,
            title: "MC_Title",
            subTitle: "MC_SubTitle",
            description: "MC_Description",
            imageUrl: "MC_Url",
            duration: 60,
            startTime: DateTime.UtcNow,
            endTime: DateTime.UtcNow.AddHours(1),
            genreList: new List<string> { "Genre1", "Genre2", "Genre3" });
    }

    public static readonly ContentInput MockUpdateContentRequest = new ContentInput
    {
        Title = null,
        SubTitle = null,
        Description = "MC_New_Description",
        ImageUrl = "MC_New_Url",
        Duration = null,
        StartTime = null,
        EndTime = null,
    };

    public static Content MockUpdateContentUpdated(Guid id)
    {
        return new Content(
            id: id,
            title: "MC_Title",
            subTitle: "MC_SubTitle",
            description: "MC_New_Description",
            imageUrl: "MC_New_Url",
            duration: 60,
            startTime: DateTime.UtcNow,
            endTime: DateTime.UtcNow.AddHours(1),
            genreList: new List<string> { "Genre1", "Genre2", "Genre3" });
    }


    public static Content MockAddGenresOriginal(Guid id)
    {
        return new Content(
            id: id,
            title: "MC_Title",
            subTitle: "MC_SubTitle",
            description: "MC_Description",
            imageUrl: "MC_Url",
            duration: 60,
            startTime: DateTime.UtcNow,
            endTime: DateTime.UtcNow.AddHours(1),
            genreList: new List<string> { "Genre1" });
    }

    public static readonly IEnumerable<string> MockAddGenresRequest = new List<string>
    {
        "Genre1",
        "Genre2",
    };

    public static Content MockAddGenresUpdated(Guid id)
    {
        return new Content(
            id: id,
            title: "MC_Title",
            subTitle: "MC_SubTitle",
            description: "MC_Description",
            imageUrl: "MC_Url",
            duration: 60,
            startTime: DateTime.UtcNow,
            endTime: DateTime.UtcNow.AddHours(1),
            genreList: new List<string> { "Genre1", "Genre2" });
    }


    public static Content MockRemoveGenresOriginal(Guid id)
    {
        return new Content(
            id: id,
            title: "MC_Title",
            subTitle: "MC_SubTitle",
            description: "MC_Description",
            imageUrl: "MC_Url",
            duration: 60,
            startTime: DateTime.UtcNow,
            endTime: DateTime.UtcNow.AddHours(1),
            genreList: new List<string> { "Genre1", "Genre2", "Genre3" });
    }

    public static readonly IEnumerable<string> MockRemoveGenresRequest = new List<string>
    {
        "Genre3",
        "Genre4",
    };

    public static Content MockRemoveGenresUpdated(Guid id)
    {
        return new Content(
            id: id,
            title: "MC_Title",
            subTitle: "MC_SubTitle",
            description: "MC_Description",
            imageUrl: "MC_Url",
            duration: 60,
            startTime: DateTime.UtcNow,
            endTime: DateTime.UtcNow.AddHours(1),
            genreList: new List<string> { "Genre1", "Genre2" });
    }


    public static IEnumerable<Content?> MockGetFilteredContentsAllContents(Guid responseId)
    {
        return new List<Content?>()
        {
            new Content(
                id: Guid.NewGuid(),
                title: "MC1_Title",
                subTitle: "MC1_SubTitle",
                description: "MC1_Description",
                imageUrl: "MC1_Url",
                duration: 60,
                startTime: DateTime.UtcNow,
                endTime: DateTime.UtcNow.AddHours(1),
                genreList: new List<string> { "Genre1", "Genre2" }),

            new Content(
                id: responseId,
                title: "MC2_Title",
                subTitle: "MC2_SubTitle",
                description: "MC2_Description",
                imageUrl: "MC2_Url",
                duration: 90,
                startTime: DateTime.UtcNow,
                endTime: DateTime.UtcNow.AddHours(1.5),
                genreList: new List<string> { "Genre2", "Genre3" }),

            new Content(
                id: Guid.NewGuid(),
                title: "MC3_Title",
                subTitle: "MC3_SubTitle",
                description: "MC3_Description",
                imageUrl: "MC3_Url",
                duration: 120,
                startTime: DateTime.UtcNow,
                endTime: DateTime.UtcNow.AddHours(2),
                genreList: new List<string> { "Genre3", "Genre4" })
        };
    }

    public static readonly string MockGetFilteredContentsTitleFilter = "MC2";
    public static readonly string MockGetFilteredContentsGenreFilter = "Genre3";

    public static IEnumerable<Content?> MockGetFilteredContentsResponse(Guid id)
    {
        return new List<Content?>()
        {
            new Content(
                id: id,
                title: "MC2_Title",
                subTitle: "MC2_SubTitle",
                description: "MC2_Description",
                imageUrl: "MC2_Url",
                duration: 90,
                startTime: DateTime.UtcNow,
                endTime: DateTime.UtcNow.AddHours(1.5),
                genreList: new List<string> { "Genre2", "Genre3" })
        };
    }
}