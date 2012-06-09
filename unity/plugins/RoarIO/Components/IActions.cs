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
 * \brief IActions is an interface for listing and executing user actions.
 *
 * Actions typically have three main lists: requirements, costs and rewards:
 * - Requirements must be met in order for task to be able to complete
 * - Costs are deducted from a user (and must be met to successfully complete the task)
 * - Rewards describes the rewards that are given to a user for completing the task
 *
 * An action can have requirements, these are found in the requires list.
 * Each requirement will come back with an "ok" tag and some extra information.
 *
 * Requirement types are:
 *  - \c level_requirement A minimum level for the player to attempt the task.
 *   - \c level
 *  - \c item_requirement An item that the player must have to attempt the task
 *   - \c ikey The ikey of the item required
 *   - \c label A human readable description of the requirement. This is needed since
 *          it is not easy/possible to have "2 keen blades of fright" pluralise automatically.
 *   - \c number_required This will often be 1, but need not be
 *   - \c consumed Boolean that determines whether this requirement should be treated as a cost,
 *          rather than just a requirement.
 *  - \c currency_requirement
 *   - \c ikey the required currency type
 *   - \c value how much is needed
 *  - \c attribute_requirement
 *   - \c ikey the required attribute type
 *   - \c value how much is needed
 *  - \c resource_requirement
 *   - \c ikey the required resource type
 *   - \c value how much is needed
 *
 * You can get the list of all actions via the #list function,
 * and execute an action via the #execute function.
 *
 * An action can be accessed via its Hashtable interface:
 *
 * \code
 *
 * Hashtable action = Actions.list()[0];
 *
 * string actionKey = action["ikey"];
 * string actionLabel = action["label"];
 * string actionDescription = action["description"];
 *
 * Hashtable requirement = action["requirements"][0];
 * string requirementType = requirement["type"];
 * string requirementIkey = requirement["ikey"];
 * string requirementValue = requirement["value"];
 *
 * Hashtable cost = action["costs"][0];
 * string costType = cost["type"];
 * string costKey = cost["ikey"];
 * string costValue = cost["value"];
 * string costStatus = cost["ok"];
 * string costReason = cost["reason"];
 *
 * Hashtable reward = action["rewards"][0];
 * string rewardType = reward["type"];
 * string rewardKey = reward["ikey"];
 * string rewardVale = reward["value"];
 *
 * Hashtable tag = action["tags"][0];
 * string tagValue = tag["value"];
 *
 * \endcode
 *
 * @note The #list functions can only be called after the actions have been fetched from the server via a call to #fetch.
 * @note once #fetch has received and processed actions from the server, the #hasDataFromServer
 * property will return true and calls to #list will be functional.
 **/
public interface IActions
{
  /**
   * Fetch user actions from the server.
   *
   * On success:
   * - invokes callback with parameter *Hastable data* containing the actions for the user
   * - sets #hasDataFromServer to true
   *
   * On failure:
   * - invokes callback with error code and error message
   *
   * @param callback the callback function to be passed this function's result.
   *
   * @returns nothing - use a callback and/or subscribe to RoarIOManager events for results of non-blocking calls.
   **/
  void fetch( Roar.Callback callback );

  /**
   * Check whether any action data has been obtained from the server.
   *
   * @returns true if #fetch has completed execution.
   */
  bool hasDataFromServer { get; }

  /**
   * Get a list of all the actions for the authenticated user.
   *
   * @returns A list of Hashtables for each action.
   *
   * @note This does _not_ make a server call. It requires the actions to
   *       have already been fetched via a call to #fetch. If this function
   *       is called prior to the successful completion of a #fetch call,
   *       it will return an empty array.
   **/
  ArrayList list();

  /**
   * Get a list of all the actions for the authenticated user.
   *
   * On success:
   * - invokes callback with parameter *data* containing the list of Hashtable actions
   *
   * On failure:
   * - returns an empty list
   *
   * @param callback the callback function to be passed this function's result.
   *
   * @returns A list of Hashtables for each action.
   *
   * @note This does _not_ make a server call. It requires the actions to
   *       have already been fetched via a call to #fetch. If this function
   *       is called prior to the successful completion of a #fetch call,
   *       it will return an empty array.
   **/
  ArrayList list(Roar.Callback callback);

  /**
   * Initiates an action on the server, which evaluates the requirements and conditions for the action.
   * @todo examine format of data returned from IActions.execute().
   * @todo document events fired by IActions.execute().
   *
   * On success:
   * - invokes callback with data param containing result of executed action
   *
   * On failure:
   * - invokes callback with error code and error message
   *
   * @param ikey the key of the action to execute.
   * @param callback the callback function to be passed this function's result.
   *
   * @returns nothing - use a callback and/or subscribe to RoarIOManager events for results of non-blocking calls.
   **/
  void execute( string ikey, Roar.Callback callback );
}

}