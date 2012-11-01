using System.Collections;

public interface IRequestCallback<T>
{
	void OnRequest (Roar.CallbackInfo<T> info);
};

// Provides a little more structure to the IRequestCallback function.
public abstract class SimpleRequestCallback<T> : IRequestCallback<T>
{
	protected Roar.Callback cb;

	public SimpleRequestCallback (Roar.Callback in_cb)
	{
		cb = in_cb;
	}

	public virtual void OnRequest (Roar.CallbackInfo<T> info)
	{
		Prologue ();
		if (info.code != 200) {
			if (cb != null)
				cb (new Roar.CallbackInfo<object> (null, info.code, info.msg));
			OnFailure (info);
		} else {
			object result = OnSuccess (info);
			if (cb != null)
				cb (new Roar.CallbackInfo<object> (result, info.code, info.msg));

		}
		Epilogue ();
	}
  
	public abstract object OnSuccess (Roar.CallbackInfo<T> info);

	public virtual void OnFailure (Roar.CallbackInfo<T> info)
	{}

	public virtual void Prologue ()
	{}

	public virtual void Epilogue ()
	{}
};

public interface IRequestSender
{
	void MakeCall (string apicall, Hashtable args, IRequestCallback<IXMLNode> cb);
}

