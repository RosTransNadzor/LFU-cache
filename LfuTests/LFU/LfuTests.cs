using System;
using ConsoleTest.LFU;
namespace LfuTests.LFU;

public class LfuTests
{
    [Fact]
    public void InsertWhenEmpty()
    {
        //Arrange
        LFU<string, int> lfu = new(1);
        var key = "key";
        //Act
        lfu.Insert(key,5);
        
        //Assert
        Assert.Equal(1,lfu.Count);

        var frequency = new ValueFrequency<int>
        {
            Frequency = 1,
            Value = 5
        };
        
        Assert.Equal(frequency,lfu.GetWithFrequency(key));
    }

    [Fact]
    public void InsertWhenKeyExists()
    {
        LFU<string, int> lfu = new(1);
        var key = "key";
        //Act
        lfu.Insert(key,5);
        //Assert
        Assert.Throws<Exception>(() => lfu.Insert(key, 4));
    }
    [Fact]
    public void InsertWhenLfuIsFull()
    {
        LFU<string, int> lfu = new(1);
        var key1 = "key1";
        var key2 = "key2";

        //Act
        lfu.Insert(key1,5);
        lfu.Insert(key2,4);
        //Assert
        var frequency = new ValueFrequency<int>
        {
            Frequency = 1,
            Value = 4
        };
        
        Assert.Equal(frequency,lfu.GetWithFrequency(key2));
        Assert.Throws<Exception>(() => lfu.GetWithFrequency(key1));
        Assert.Equal(1,lfu.Count);
    }

    [Fact]
    public void AccessUpdateFrequency()
    {
        //Arrange
        LFU<string, int> lfu = new(1);
        var key = "key";
        //Act
        lfu.Insert(key,5);
        lfu.Access(key);
        
        //Assert
        Assert.Equal(1,lfu.Count);

        var frequency = new ValueFrequency<int>
        {
            Frequency = 2,
            Value = 5
        };
        
        Assert.Equal(frequency,lfu.GetWithFrequency(key));
    }

    [Fact]
    public void InsertRemoveItemWithLowestFrequency()
    {
        //Arrange
        LFU<string, int> lfu = new(2);
        var key1 = "key1";
        var key2 = "key2";
        var key3 = "key3";
        //Act
        lfu.Insert(key1,1);
        lfu.Insert(key2,2);
        lfu.Access(key2);
        lfu.Insert(key3,3);
        
        //Assert
        Assert.Throws<Exception>(() => lfu.GetWithFrequency(key1));
        var frequency = new ValueFrequency<int>
        {
            Frequency = 2,
            Value = 2
        };
        
        Assert.Equal(frequency,lfu.GetWithFrequency(key2));
    }

    [Fact]
    public void RemoveLastAdded()
    {
        //Arrange
        LFU<string, int> lfu = new(2);
        var key1 = "key1";
        var key2 = "key2";
        var key3 = "key3";
        
        //Act
        lfu.Insert(key1,1);
        lfu.Insert(key2,2);
        lfu.Insert(key3,3);
        
        //Assert
        Assert.Throws<Exception>(() => lfu.GetWithFrequency(key1));
    }
    [Fact]
    public void RemoveFirstAdded()
    {
        //Arrange
        LFU<string, int> lfu = new(2,AddPolicy.AddToHead);
        var key1 = "key1";
        var key2 = "key2";
        var key3 = "key3";
        
        //Act
        lfu.Insert(key1,1);
        lfu.Insert(key2,2);
        lfu.Insert(key3,3);
        
        //Assert
        Assert.Throws<Exception>(() => lfu.GetWithFrequency(key2));
    }
}