/*
Copyright (c) 2012, Run With Robots
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the roar.io library nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY RUN WITH ROBOTS ''AS IS'' AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL MICHAEL ANDERSON BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using DC = Roar.implementation.DataConversion;
using System.Collections;

namespace Roar.implementation
{

public class DataStore
{
  public DataStore(IRequestSender api, ILogger logger)
  {
    Properties_   = new DataModel("properties", "user/view", "attribute", null, new DC.XmlToPropertyHashtable(), api, logger);
    Inventory_    = new DataModel("inventory", "items/list", "item", null, new DC.XmlToInventoryItemHashtable(), api, logger);
    Shop_         = new DataModel("shop", "shop/list", "shopitem", null, new DC.XmlToShopItemHashtable(), api, logger);
    Actions_      = new DataModel("tasks", "tasks/list", "task", null, new DC.XmlToTaskHashtable(), api, logger);
    Gifts_        = new DataModel("gifts", "mail/what_can_i_send", "mailable", null, new DC.XmlToGiftHashtable(), api, logger);
    Achievements_ = new DataModel("achievements", "user/achievements", "achievement", null, new DC.XmlToAchievementHashtable(), api, logger);
    Leaderboards_ = new DataModel("leaderboards", "leaderboards/list", "board", null, null, api, logger);
    Friends_      = new DataModel("friends", "friends/list", "friend", null, null, api, logger);
    Cache_        = new ItemCache("cache", "items/view", "item", null, new DC.XMLToItemHashtable(), api, logger);
    Appstore_     = new DataModel("appstore", "appstore/shop_list", "shopitem", null, new DC.XmlToAppstoreItemHashtable(), api, logger);
  }

  public void clear(bool x)
  {
    Properties_.clear(x);
    Inventory_.clear(x);
    Shop_.clear(x);
    Actions_.clear(x);
    Gifts_.clear(x);
    Achievements_.clear(x);
    Leaderboards_.clear(x);
    Friends_.clear(x);
    Cache_.clear(x);
    Appstore_.clear(x);
  }

  public DataModel Properties_;
  public DataModel Inventory_;
  public DataModel Shop_;
  public DataModel Actions_;
  public DataModel Gifts_;
  public DataModel Achievements_;
  public DataModel Leaderboards_;
  public DataModel Friends_;
  public DataModel Appstore_;
  public ItemCache Cache_;
}

}