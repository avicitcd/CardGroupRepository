/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 3.0.2
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */

namespace acro {

public class DataReceiver : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal DataReceiver(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(DataReceiver obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~DataReceiver() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          libacro1626pPINVOKE.delete_DataReceiver(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public void startReceive(string strJsonReveive, DataUpdateCallback callback) {
    libacro1626pPINVOKE.DataReceiver_startReceive__SWIG_0(swigCPtr, strJsonReveive, DataUpdateCallback.getCPtr(callback));
    if (libacro1626pPINVOKE.SWIGPendingException.Pending) throw libacro1626pPINVOKE.SWIGPendingException.Retrieve();
  }

  public void startReceive(string strJsonReveive) {
    libacro1626pPINVOKE.DataReceiver_startReceive__SWIG_1(swigCPtr, strJsonReveive);
    if (libacro1626pPINVOKE.SWIGPendingException.Pending) throw libacro1626pPINVOKE.SWIGPendingException.Retrieve();
  }

  public void stopReceive() {
    libacro1626pPINVOKE.DataReceiver_stopReceive(swigCPtr);
  }

  public DataReceiver() : this(libacro1626pPINVOKE.new_DataReceiver(), true) {
  }

}

}