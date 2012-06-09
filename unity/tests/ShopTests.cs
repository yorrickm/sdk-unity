using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NMock2;
using Roar.Components;

/**
 * Test cases for the Shop component.
 **/
[TestFixture]
public class ShopTests : ComponentTests
{
  protected IShop shop;
  
  // returned by server when shop items are fetched.
  public static string shopList = 
@"<roar tick='130695522924'>
  <shop>
    <list status='ok'>
      <shopitem ikey='shop_item_ikey_1' label='Shop item 1' description='Lorem Ipsum'>
        <costs>
          <stat_cost type='currency' ikey='cash' value='100' ok='false' reason='Insufficient Coins'/>
          <stat_cost type='currency' ikey='premium_currency' value='0' ok='true'/>
        </costs>
        <modifiers>
          <grant_item ikey='item_ikey_1'/>
        </modifiers>
        <tags/>
      </shopitem>
      <shopitem ikey='shop_item_ikey_2'/>
      <shopitem ikey='shop_item_ikey_3' label='Shop item 2'/>
      <shopitem ikey='shop_item_ikey_4' description='Blah Blah'>
        <costs>
          <stat_cost type='currency' ikey='cash' value='0'/>
          <stat_cost type='currency' ikey='premium_currency' value='50'/>
        </costs>
        <modifiers>
          <grant_item ikey='item_ikey_2'/>
        </modifiers>
        <tags>
          <tag value='a_tag'/>
          <tag value='another_tag'/>
        </tags>
      </shopitem>
    </list>
  </shop>
</roar>";

  [SetUp]
  new public void TestInitialise()
  {
    base.TestInitialise();
    shop = roar.Shop;
    Assert.IsNotNull(shop);
    Assert.IsFalse(shop.hasDataFromServer);
  }
  
  protected void mockFetch(string mockResponse, Roar.Callback cb) {
    requestSender.addMockResponse("shop/list", mockResponse);
    // todo: mock a response from items/view for testing the item cache
    requestSender.addMockResponse("items/view", " ");
    shop.fetch(cb);
  }
  
  [Test]
  public void testFetchSuccess() {
    bool callbackExecuted = false;
    Roar.Callback roarCallback = (Roar.CallbackInfo callbackInfo) => { 
      callbackExecuted=true;
      Assert.AreEqual(IWebAPI.OK, callbackInfo.code);
      Assert.IsNotNull(callbackInfo.d);
    };
    mockFetch(shopList, roarCallback);
    Assert.IsTrue(callbackExecuted);
    Assert.IsTrue(shop.hasDataFromServer);
  }

  [Test]
  [Ignore]
  public void testFetchFailureServerDown() {
    //assertions:
    //callback called with expected error code
    //hasDataFromServer == false
  }

  [Test]
  public void testList() {

    mockFetch(shopList, null);
    Assert.IsTrue(shop.hasDataFromServer);
    
    //returns a list of shop items with the expected data structure
    int expectedItemCount = 4;
    ArrayList itemHashtables = shop.list();
    Assert.AreEqual(expectedItemCount, itemHashtables.Count);
    
    //invokes callback with parameter *data* containing the list of Hashtable shop items
    bool callbackExecuted = false;
    Roar.Callback roarCallback = (Roar.CallbackInfo callbackInfo) => { 
      callbackExecuted=true;
      Assert.AreEqual(IWebAPI.OK, callbackInfo.code);
      Assert.IsNotNull(callbackInfo.d);
      Assert.AreEqual(callbackInfo.d, itemHashtables);
    };
    itemHashtables = shop.list(roarCallback);
    Assert.IsTrue(callbackExecuted);
    Assert.AreEqual(expectedItemCount, itemHashtables.Count);
  }

  [Test]
  [Ignore]
  // TODO: modifiers is returning null - does this not get translated from the xml?
  public void testGetShopItem() {
    
    //returns null on no data from server
    Assert.IsNull(shop.getShopItem("shop_item_ikey_1"));
    
    mockFetch(shopList, null);
    
    //returns Hashtable of property if exists
    Hashtable shopItem = shop.getShopItem("shop_item_ikey_1") as Hashtable;
    ArrayList costs = shopItem["costs"] as ArrayList;
    Hashtable costA = costs[0] as Hashtable;
    Hashtable costB = costs[1] as Hashtable;
    StringAssert.IsMatch("cash", costA["ikey"] as String);
    StringAssert.IsMatch("premium_currency", costB["ikey"] as String);
    Assert.AreEqual(false, (bool)costA["ok"]);
    Assert.AreEqual(true, (bool)costB["ok"]);
    
    ArrayList modifiers = shopItem["modifiers"] as ArrayList;
    Hashtable modifier = modifiers[0] as Hashtable;
    StringAssert.IsMatch("item_ikey_1", modifier["ikey"] as String);

    //returns null on property not existing
    Assert.IsNull(shop.getShopItem("doesnotexist"));
  }
        
  [Test]
  [Ignore]
  public void testBuy() {
    //assertions:
    //correct event triggered with expected arguments (GoodInfo)
    //callback executed with expected arguments (GoodInfo)
  }
 
  [Test]
  [Ignore]
  public void testBuyFailureServerDown() {
    //assertions:
    //callback called with expected error code
  }
}

