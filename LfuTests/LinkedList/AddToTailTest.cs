using ConsoleTest.LFU;

namespace LfuTests;

public class AddToTailTests
{
    [Fact]
    public void AddToTailWhenEmpty()
    {
        // Arrange
        LinkedList<int> nums = new();

        // Act
        nums.AddNodeToTail(1);

        // Assert
        Assert.NotNull(nums.Tail);
        Assert.Equal(1, nums.Tail.Value);
        
        Assert.Same(nums.Head, nums.Tail);
    }

    [Fact]
    public void AddToTailWhenHasOneElement()
    {
        // Arrange
        LinkedList<int> nums = new();
        nums.AddNodeToTail(1);

        // Act
        nums.AddNodeToTail(2);

        // Assert
        Assert.NotNull(nums.Head);
        Assert.Equal(1, nums.Head.Value);  
        
        Assert.NotNull(nums.Tail);
        Assert.Equal(2, nums.Tail.Value);

        // Проверка корректности связей
        Assert.Same(nums.Head.Next, nums.Tail);
        Assert.Same(nums.Tail.Prev, nums.Head);
    }

    [Fact]
    public void AddToTailWhenHasMoreThanOneElement()
    {
        // Arrange
        LinkedList<int> nums = new();
        nums.AddNodeToTail(1);
        nums.AddNodeToTail(2);

        // Act
        nums.AddNodeToTail(3);

        // Assert
        Assert.NotNull(nums.Head);
        Assert.Equal(1, nums.Head.Value); 
        
        Assert.NotNull(nums.Tail);
        Assert.Equal(3, nums.Tail.Value); 
        
        var middle = nums.Head.Next;

        Assert.NotNull(middle);
        Assert.Equal(2, middle.Value); 
        
        Assert.Same(middle.Next, nums.Tail);
        Assert.Same(nums.Tail.Prev, middle);
    }
}