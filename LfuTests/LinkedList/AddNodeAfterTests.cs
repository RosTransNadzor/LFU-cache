using ConsoleTest.LFU;

namespace LfuTests;

public class AddNodeAfterTests
{
    [Fact]
    public void AddNodeAfter()
    {
        //Arrange
        LinkedList<int> nums = new();
        nums.AddNodeToTail(1);
        Node<int> insertNode = new Node<int>
        {
            Value = 2
        };
        
        nums.AddNodeToTail(insertNode);
        nums.AddNodeToTail(3);
        
        //Act
        Node<int> newNode = new Node<int>
        {
            Value = 4
        };
        nums.AddNodeAfter(insertNode,newNode);
        
        //Assert
        
        Assert.Same(newNode.Next,nums.Tail);
        Assert.Same(newNode.Prev,insertNode);
        
        Assert.Same(insertNode.Next,newNode);
        Assert.Same(nums.Tail!.Prev,newNode);

    }

    [Fact]
    public void AddNodeAfterHead()
    {
        //Arrange
        LinkedList<int> nums = new();
        nums.AddNodeToTail(1);
        var added = new Node<int>
        {
            Value = 4
        };
        
        //Act
        nums.AddNodeAfter(nums.Head!,added);
        
        //Assert
        Assert.NotNull(nums.Head);
        Assert.Equal(1,nums.Head.Value);
        Assert.Same(nums.Head.Next,added);
        Assert.Same(nums.Tail,added);
        Assert.Same(nums.Tail!.Prev,nums.Head);
    }
}