using System.Collections;




public interface IRequestCallback<T>
{
  void onRequest( Roar.CallbackInfo<T> info );
};

// Provides a little more structure to the IRequestCallback function.
public abstract class SimpleRequestCallback<T> : IRequestCallback<T>
{
  protected Roar.Callback cb;
  public SimpleRequestCallback( Roar.Callback in_cb )
  {
    cb = in_cb;
  }
  public virtual void onRequest( Roar.CallbackInfo<T> info )
  {
    prologue();
    if (info.code!=200)
    {
      if (cb!=null) cb(new Roar.CallbackInfo<object>(null,info.code,info.msg));
      onFailure( info );
    }
    else
    {
      object result = onSuccess(info);
      if (cb!=null) cb( new Roar.CallbackInfo<object>(result, info.code, info.msg) );

    }
    epilogue();
  }
  
  public abstract object onSuccess( Roar.CallbackInfo<T> info );
  public virtual void onFailure( Roar.CallbackInfo<T> info ) {}
  public virtual void prologue() {}
  public virtual void epilogue() {}
};

public interface IRequestSender
{
  void make_call( string apicall, Hashtable args, IRequestCallback<IXMLNode> cb );
}

