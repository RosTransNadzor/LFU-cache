using ConsoleTest.LFU;
namespace LfuTests;

public class AddToHeadTests
{
    [Fact]
    public void AddToHeadWhenEmpty()
    {
        LinkedList<int> nums = new();
        nums.AddNodeToHead(1);

        Assert.NotNull(nums.Head);
        Assert.Equal(1, nums.Head.Value);
        
        Assert.Same(nums.Head, nums.Tail);
    }

    [Fact]
    public void AddToHeadWhenHasOneElement()
    {
        LinkedList<int> nums = new();
        nums.AddNodeToHead(1);
        nums.AddNodeToHead(2);

        Assert.NotNull(nums.Head);
        Assert.Equal(2, nums.Head.Value);

        Assert.NotNull(nums.Tail);
        Assert.Equal(1, nums.Tail.Value);
        
        Assert.Same(nums.Head.Next, nums.Tail);
        Assert.Same(nums.Tail.Prev, nums.Head);
        
    }

    [Fact]
    public void AddToHeadWhenHasMoreThanOneElement()
    {
        LinkedList<int> nums = new();
        nums.AddNodeToHead(1);
        nums.AddNodeToHead(2);
        nums.AddNodeToHead(3);

        Assert.NotNull(nums.Head);
        Assert.Equal(3, nums.Head.Value);

        Assert.NotNull(nums.Tail);
        Assert.Equal(1, nums.Tail.Value);
        
        var middle = nums.Head.Next;

        Assert.NotNull(middle);
        Assert.Equal(2, middle.Value);

        Assert.Same(nums.Tail.Prev, middle);
        Assert.Same(middle.Prev, nums.Head);
        Assert.Same(middle.Next, nums.Tail);
    }
    
}