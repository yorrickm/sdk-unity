using System;
using System.Collections;

/**
 * This acts as a simple bridge to pull in the one function we need from
 * unity objects into the RequestSender class.
 */
public interface IUnityObject
{
	void DoCoroutine(IEnumerator methodName);
}
