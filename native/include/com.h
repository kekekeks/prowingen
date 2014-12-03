#ifndef COM_H_INCLUDED
#define COM_H_INCLUDED

#include <cstring>

typedef struct _GUID {
    unsigned int  Data1;
    unsigned short Data2;
    unsigned short Data3;
    unsigned char  Data4[ 8 ];
} GUID;
typedef GUID IID;
typedef const IID* REFIID;
typedef unsigned int HRESULT;
typedef unsigned int DWORD;
typedef DWORD ULONG;

#define STDMETHODCALLTYPE

#define S_OK                             0x0L

#define E_NOINTERFACE                    0x80004002L


typedef struct _IUnknown
{
    virtual HRESULT STDMETHODCALLTYPE QueryInterface(
            REFIID riid,
            void **ppvObject) = 0;

    virtual ULONG STDMETHODCALLTYPE AddRef( void) = 0;

    virtual ULONG STDMETHODCALLTYPE Release( void) = 0;

} IUnknown;

template<class TInterface, GUID const* TIID> class ComObject :  public TInterface
{

private:
    unsigned int _refCount;
public:

    virtual ULONG AddRef()
    {
        _refCount++;
        return _refCount;
    }


    virtual ULONG Release()
    {
        _refCount--;
        ULONG rv = _refCount;
        if(_refCount == 0)
            delete(this);
        return rv;
    }

    virtual HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid,
            void **ppvObject)
    {
        const GUID IID_IUnknown = { 0x00, 0x0, 0x0, { 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46 } };
        if(0 == memcmp(riid, &IID_IUnknown, sizeof(GUID)))
            *ppvObject = (IUnknown*)this;
        else if(0 == memcmp(riid, TIID, sizeof(GUID)))
        {
            TInterface* casted = this;
            *ppvObject = casted;
        }
        else
            return E_NOINTERFACE;
        _refCount++;
        return S_OK;
    }
    ComObject()
    {
        _refCount = 1;

    }
    virtual ~ComObject()
    {
    }
};

template<class TInterface>
class ComPtr
{
private:
    TInterface* _obj;
public:
    ComPtr(TInterface* pObj)
    {
        _obj = 0;

        if (pObj)
        {
            _obj = pObj;
            _obj->AddRef();
        }
    }

    ~ComPtr()
    {
        if (_obj)
        {
            _obj->Release();
            _obj = 0;
        }
    }

public:
    operator TInterface*() const
    {
        return _obj;
    }
    TInterface& operator*() const
    {
        return *_obj;
    }
    TInterface** operator&()
    {
        return &_obj;
    }
    TInterface* operator->() const
    {
        return _obj;
    }
};


#endif // COM_H_INCLUDED
