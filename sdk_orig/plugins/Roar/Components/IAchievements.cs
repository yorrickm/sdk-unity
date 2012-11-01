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
using System.Collections;

namespace Roar.Components
{

  /**
   * \brief IAchievements is an interface for listing all of a user's achievements.
   *
   * Achievements are created in Roar by setting a task and defining a number of completion
   * events. Once a user has completed the tasks the prescribed number of times, the achievement
   * is rewarded. 
   * 
   * You can get the list of all achievements via the #list function or
   * a single achievement via the #getAchievement function.
   *
   * An achievement is represented in a Hashtable using the following keys:
   *
   * \code
   *   - ikey : "the_big_one"
   *   - status : "active"
   *   - label : "The Big One"
   *   - progress : "0/3"
   *   - description "Find the dragon three times!"
   *   - task_ikey : "an_ikey"
   *   - task_label : "A task label"
   * \endcode
   *
   **/
  public interface IAchievements
  {

    /**
     * Fetch achievement information from the server.
     *
     * On success:
     * - invokes callback with parameter *Hastable data* containing the achievements for the user
     * - fires the RoarManager#achievementsReadyEvent
     * - sets #hasDataFromServer to true
     *
     * On failure:
     * - invokes callback with error code and error message
     *
     * @param callback the callback function to be passed this function's result.
     *
     * @returns nothing - use a callback and/or subscribe to RoarManager events for results of non-blocking calls.
     **/
    void fetch (Roar.Callback callback);

    /**
     * Check whether any user achievements data has been obtained from the server.
     *
     * @returns true if #fetch has completed execution.
     **/
    bool hasDataFromServer { get; }

    /**
     * Get a list of all the achievement objects for the authenticated user.
     *
     * @returns A list of Hashtables for each user achievement.
     *
     * @note This does _not_ make a server call. It requires the user achievements to
     *       have already been fetched via a call to #fetch. If this function
     *       is called prior to the successful completion of a #fetch call,
     *       it will return an empty array.
     **/
    ArrayList list ();

    /**
     * Get a list of all the achievement objects for the authenticated user.
     *
     * On success:
     * - invokes callback with parameter *data* containing the list of Hashtable user achievements
     *
     * On failure:
     * - returns an empty list
     *
     * @param callback the callback function to be passed this function's result.
     *
     * @returns A list of Hashtables for each user achievement.
     *
     * @note This does _not_ make a server call. It requires the user achievements to
     *       have already been fetched via a call to #fetch. If this function
     *       is called prior to the successful completion of a #fetch call,
     *       it will return an empty array.
     **/
    ArrayList list (Roar.Callback callback);
    
    
    /**
     * Returns the achievement object for a given key.
     *
     * @param ikey the key that uniquely identifies an achievement.
     *
     * @returns the property Hashtable associated with the *key*
     *          or null if the achievement does not exist in the data store.
     **/
    Hashtable getAchievement (string ikey);

    /**
     * Returns the achievement object for a given key.
     *
     * On success:
     * - invokes callback with parameter *data* containing the achievement Hashtable
     *
     * On failure:
     * - invokes callback with parameter *data* equalling null if achievement does not exist
     *
     * @param ikey the key that uniquely identifies an achievement.
     * @param callback the callback function to be passed this function's result.
     *
     * @returns the achievement Hashtable associated with the *ikey*
     *          or null if the achievement does not exist in the data store.
     **/
    Hashtable getAchievement (string ikey, Roar.Callback callback);
  }
}
