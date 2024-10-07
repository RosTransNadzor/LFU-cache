using ConsoleTest.LFU;

namespace LfuTests;

public class RemoveNodeTests
{
    [Fact]
    public void RemoveNodeWhenHasOneElement()
    {
        //Arrange
        LinkedList<int> nums = new();
        nums.AddNodeToHead(1);
        
        //Act
        var nodeToRemove = nums.Head!;
        nums.RemoveNode(nodeToRemove);
        
        //Assert
        Assert.Null(nums.Head);
        Assert.Null(nums.Tail);
    }

    [Fact]
    public void RemoveHeadWhenHasTwoElements()
    {
        //Arrange
        LinkedList<int> nums = new();
        nums.AddNodeToHead(2);
        nums.AddNodeToHead(1);
        
        //Act
        var nodeToRemove = nums.Head!;
        nums.RemoveNode(nodeToRemove);

        //Assert
        Assert.NotNull(nums.Tail);
        Assert.Same(nums.Head,nums.Tail);
        Assert.Null(nums.Tail.Prev);
        Assert.Equal(2, nums.Tail.Value);
    }
    [Fact]
    public void RemoveTailWhenHasTwoElements()
    {
        //Arrange
        LinkedList<int> nums = new();
        nums.AddNodeToHead(2);
        nums.AddNodeToHead(1);
        
        //Act
        var nodeToRemove = nums.Tail!;
        nums.RemoveNode(nodeToRemove);

        //Assert
        Assert.NotNull(nums.Tail);
        Assert.Same(nums.Head,nums.Tail);
        Assert.Null(nums.Tail.Prev);
        Assert.Equal(1, nums.Tail.Value);
    }

    [Fact]
    public void RemoveNodeWhenHasThreeElements()
    {
        //Arrange
        LinkedList<int> nums = new();
        nums.AddNodeToHead(3);
        nums.AddNodeToHead(2);
        nums.AddNodeToHead(1);
        
        //Act
        var nodeToRemove = nums.Head!.Next!;
        nums.RemoveNode(nodeToRemove);

        //Assert
        Assert.NotNull(nums.Head);
        Assert.NotNull(nums.Tail);
        
        Assert.Same(nums.Head.Next,nums.Tail);
        Assert.Same(nums.Tail.Prev,nums.Head);
        
        Assert.Equal(1, nums.Head.Value);
        Assert.Equal(3, nums.Tail.Value);
    }
}