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
        lfu.Add(key,5);
        
        //Assert
        Assert.Single(lfu);

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
        lfu.Add(key,5);
        //Assert
        Assert.Throws<ArgumentException>(() => lfu.Add(key, 4));
    }
    [Fact]
    public void InsertWhenLfuIsFull()
    {
        LFU<string, int> lfu = new(1);
        var key1 = "key1";
        var key2 = "key2";

        //Act
        lfu.Add(key1,5);
        lfu.Add(key2,4);
        //Assert
        var frequency = new ValueFrequency<int>
        {
            Frequency = 1,
            Value = 4
        };
        
        Assert.Equal(frequency,lfu.GetWithFrequency(key2));
        Assert.Throws<ArgumentException>(() => lfu.GetWithFrequency(key1));
        Assert.Single(lfu);
    }

    [Fact]
    public void AccessUpdatesFrequency()
    {
        //Arrange
        LFU<string, int> lfu = new(1);
        var key = "key";
        //Act
        lfu.Add(key,5);
        lfu.Access(key);
        
        //Assert
        Assert.Single(lfu);

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
        lfu.Add(key1,1);
        lfu.Add(key2,2);
        lfu.Access(key1);
        lfu.Add(key3,3);
        
        //Assert
        Assert.Throws<ArgumentException>(() => lfu.GetWithFrequency(key2));
        var frequency = new ValueFrequency<int>
        {
            Frequency = 2,
            Value = 1
        };
        
        Assert.Equal(frequency,lfu.GetWithFrequency(key1));
    }

    [Fact]
    public void RemoveLastAddedByInsert()
    {
        //Arrange
        //AddPolicy.AddToTail used by default
        LFU<string, int> lfu = new(2);
        var key1 = "key1";
        var key2 = "key2";
        var key3 = "key3";
        
        //Act
        lfu.Add(key1,1);
        lfu.Add(key2,2);
        lfu.Add(key3,3);
        
        //Assert
        Assert.Throws<ArgumentException>(() => lfu.GetWithFrequency(key1));
    }
    [Fact]
    public void RemoveFirstAddedByInsert()
    {
        //Arrange
        LFU<string, int> lfu = new(2,AddPolicy.AddToHead);
        var key1 = "key1";
        var key2 = "key2";
        var key3 = "key3";
        
        //Act
        lfu.Add(key1,1);
        lfu.Add(key2,2);
        lfu.Add(key3,3);
        
        //Assert
        Assert.Throws<ArgumentException>(() => lfu.GetWithFrequency(key2));
    }

    [Fact]
    public void DeleteItemFromCache()
    {
        //Assert
        LFU<string, int> lfu = new(2);
        var key1 = "key1";
        var key2 = "key2";
        //Act
        lfu.Add(key1,1);
        lfu.Access(key1);
        lfu.Add(key2,2);
        lfu.Remove(key1);
        
        //Assert
        Assert.Throws<ArgumentException>(() => lfu.GetWithFrequency(key1));
    }
}