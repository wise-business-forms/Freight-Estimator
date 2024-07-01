using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Config
/// </summary>
public static class Config
{
    static string _UPSAccessKey = "CC83ED82D080DC80";
    static string _UPSUserName = "WiseWebSupport";
    static string _UPSPassword = "Wise_forms";//"Nextwave1";//"wise_forms";

    static string _ShipFromShipperNumber = "391287";

    static string _NetworkDomain = "WISENT";

    //static string[] _PlantCodes = { "ALP", "BUT", "FTW", "PDT", "POR", "BMK", "COR" };
    static string[] _PlantCodes = { "ALP", "BUT", "FTW", "POR", "BMK", "COR", "CIN", "EBT" };
    static string[] _PlantCodesMultiRate = { "ALP", "BUT", "FTW", "POR", "BMK" };

    static double _MinCWTWeightGround = 200;
    static double _MinCWTPackagesGround = 2;

    static double _MinCWTWeightAir = 100;
    static double _MinCWTPackagesAir = 2;

    static Dictionary<string, string> _PlantNames = new Dictionary<string, string>();

    static string _M33DemoUrl = "http://demo.m33integrated.com/api/";
    static string _M33ProdUrl = "https://blackbeardapp.com/api/";

    static string _M33DemoToken = "696c42d819642885724b60ffcb7d636deadd632e";
    static string _M33ProdToken = "d579620ffba756e5c2ec9f76e3447f98bf85770b";

    static bool _M33DemoMode = false;

    static string _TransPlaceDemoUrl = "https://uattms.transplace.com/xml-api/api/";
    static string _TransPlaceProdUrl = "https://tms.transplace.com/xml-api/api/";

    static string _TransPlaceDemoToken = "8Hhk8etqxs94gEtj0JpVxl90qoINDfMAiemh84XbGNM%3D";
    //static string _TransPlaceProdToken = "jLDnfupyBDGy%2FnF2zYs4g5Zl0%2B7g7TBLPIwBuNfAVhc%3D";
    static string _TransPlaceProdToken = "jLDnfupyBDGy%2FnF2zYs4gxMB78GTlIAytU2dzN4xQLdVWNGGmkNOl%2B9N%2BU6aO4vP";

    static bool _TransPlaceDemoMode = false;


    static Config() {
        populatePlantNames();
    }

    private static void populatePlantNames() {
        _PlantNames.Add("ALP", "Alpharetta");
        _PlantNames.Add("BUT", "Butler");
        _PlantNames.Add("FTW", "Ft Wayne");
        _PlantNames.Add("PDT", "Piedmont");
        _PlantNames.Add("POR", "Portland");
        _PlantNames.Add("BMK", "Brandmark");
        _PlantNames.Add("COR", "Corporate");
        _PlantNames.Add("CIN", "Cincinnati");
        _PlantNames.Add("EBT", "East Butler");

    }
    
    public static string UPSAccessKey
    {
        get { return _UPSAccessKey; }
    }
    public static string UPSUserName
    {
        get { return _UPSUserName; }
    }
    public static string UPSPassword
    {
        get { return _UPSPassword; }
    }
    public static string ShipFromShipperNumber
    {
        get { return _ShipFromShipperNumber; }
    }
    public static string NetworkDomain
    {
        get { return _NetworkDomain; }
    }
    public static string[] PlantCodes
    {
        get { return _PlantCodes; }
    }
    public static string[] PlantCodesMultiRate
    {
        get { return _PlantCodesMultiRate; }
    }
    public static double MinCWTWeightGround
    {
        get { return _MinCWTWeightGround; }
    }
    public static double MinCWTPackagesGround
    {
        get { return _MinCWTPackagesGround; }
    }
    public static double MinCWTWeightAir
    {
        get { return _MinCWTWeightAir; }
    }
    public static double MinCWTPackagesAir
    {
        get { return _MinCWTPackagesAir; }
    }
    public static Dictionary<string, string> PlantNames
    {
        get { return _PlantNames; }
    }
    public static string M33Url
    {
        get 
        {
            if (_M33DemoMode)
            {
                return _M33DemoUrl;
            }
            else
            {
                return _M33ProdUrl;
            }
        }
    }
    public static string M33Token
    {
        get
        {
            if (_M33DemoMode)
            {
                return _M33DemoToken;
            }
            else
            {
                return _M33ProdToken;
            }
        }
    }


    public static string TransPlaceUrl
    {
        get
        {
            if (_TransPlaceDemoMode)
            {
                return _TransPlaceDemoUrl;
            }
            else
            {
                return _TransPlaceProdUrl;
            }
        }
    }
    public static string TransPlaceToken
    {
        get
        {
            if (_TransPlaceDemoMode)
            {
                return _TransPlaceDemoToken;
            }
            else
            {
                return _TransPlaceProdToken;
            }
        }
    }

}