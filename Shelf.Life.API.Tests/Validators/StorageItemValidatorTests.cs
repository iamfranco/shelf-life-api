using Shelf.Life.API.Validators;
using Shelf.Life.API.Validators.Models;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Stores;

namespace Shelf.Life.API.Tests.Validators;
public class StorageItemValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly AutoMocker _autoMocker = new();

    private readonly StorageItemValidator _subject;

    public StorageItemValidatorTests()
    {
        _subject = _autoMocker.CreateInstance<StorageItemValidator>();
    }

    [Fact]
    public void GivenMatchingStorageItemExists_WhenThrowIfStorageItemDoesNotExist_ThenNoAction()
    {
        //Given
        var id = _fixture.Create<int>();

        var matchingStorageItem = _fixture.Create<StorageItem>();
        _autoMocker.GetMock<IStorageItemStore>()
            .Setup(x => x.FindById(id))
            .Returns(matchingStorageItem);

        //When Then
        var act = () => _subject.ThrowIfStorageItemDoesNotExist(id);
        act.Should().NotThrow<Exception>();
    }

    [Fact]
    public void GivenNoMatchingStorageItemExist_WhenThrowIfStorageItemDoesNotExist_ThenThrowsNotFoundException()
    {
        //Given
        var id = _fixture.Create<int>();

        //When Then
        var act = () => _subject.ThrowIfStorageItemDoesNotExist(id);
        act.Should().Throw<NotFoundException>()
            .WithMessage($"{nameof(StorageItem)} with id [{id}] does NOT exist.");
    }
}
