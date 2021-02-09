namespace BotFramework
{
    internal static class Resources
    {
        private static global::System.Resources.ResourceManager s_resourceManager;
        internal static global::System.Resources.ResourceManager ResourceManager => s_resourceManager ?? (s_resourceManager = new global::System.Resources.ResourceManager(typeof(Resources)));
        internal static global::System.Globalization.CultureInfo Culture { get; set; }
#if !NET20
        [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal static string GetResourceString(string resourceKey, string defaultValue = null) =>  ResourceManager.GetString(resourceKey, Culture);

        private static string GetResourceString(string resourceKey, string[] formatterNames)
        {
           var value = GetResourceString(resourceKey);
           if (formatterNames != null)
           {
               for (var i = 0; i < formatterNames.Length; i++)
               {
                   value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
               }
           }
           return value;
        }

        /// <summary>'{0}' is not available.</summary>
        internal static string @Exception_UseMiddlewareIServiceProviderNotAvailable => GetResourceString("Exception_UseMiddlewareIServiceProviderNotAvailable");
        /// <summary>'{0}' is not available.</summary>
        internal static string FormatException_UseMiddlewareIServiceProviderNotAvailable(object p0)
           => string.Format(Culture, GetResourceString("Exception_UseMiddlewareIServiceProviderNotAvailable"), p0);

        /// <summary>No public '{0}' or '{1}' method found for middleware of type '{2}'.</summary>
        internal static string @Exception_UseMiddlewareNoInvokeMethod => GetResourceString("Exception_UseMiddlewareNoInvokeMethod");
        /// <summary>No public '{0}' or '{1}' method found for middleware of type '{2}'.</summary>
        internal static string FormatException_UseMiddlewareNoInvokeMethod(object p0, object p1, object p2)
           => string.Format(Culture, GetResourceString("Exception_UseMiddlewareNoInvokeMethod"), p0, p1, p2);

        /// <summary>'{0}' or '{1}' does not return an object of type '{2}'.</summary>
        internal static string @Exception_UseMiddlewareNonTaskReturnType => GetResourceString("Exception_UseMiddlewareNonTaskReturnType");
        /// <summary>'{0}' or '{1}' does not return an object of type '{2}'.</summary>
        internal static string FormatException_UseMiddlewareNonTaskReturnType(object p0, object p1, object p2)
           => string.Format(Culture, GetResourceString("Exception_UseMiddlewareNonTaskReturnType"), p0, p1, p2);

        /// <summary>The '{0}' or '{1}' method's first argument must be of type '{2}'.</summary>
        internal static string @Exception_UseMiddlewareNoParameters => GetResourceString("Exception_UseMiddlewareNoParameters");
        /// <summary>The '{0}' or '{1}' method's first argument must be of type '{2}'.</summary>
        internal static string FormatException_UseMiddlewareNoParameters(object p0, object p1, object p2)
           => string.Format(Culture, GetResourceString("Exception_UseMiddlewareNoParameters"), p0, p1, p2);

        /// <summary>Multiple public '{0}' or '{1}' methods are available.</summary>
        internal static string @Exception_UseMiddleMutlipleInvokes => GetResourceString("Exception_UseMiddleMutlipleInvokes");
        /// <summary>Multiple public '{0}' or '{1}' methods are available.</summary>
        internal static string FormatException_UseMiddleMutlipleInvokes(object p0, object p1)
           => string.Format(Culture, GetResourceString("Exception_UseMiddleMutlipleInvokes"), p0, p1);

        /// <summary>The path in '{0}' must start with '/'.</summary>
        internal static string @Exception_PathMustStartWithSlash => GetResourceString("Exception_PathMustStartWithSlash");
        /// <summary>The path in '{0}' must start with '/'.</summary>
        internal static string FormatException_PathMustStartWithSlash(object p0)
           => string.Format(Culture, GetResourceString("Exception_PathMustStartWithSlash"), p0);

        /// <summary>Unable to resolve service for type '{0}' while attempting to Invoke middleware '{1}'.</summary>
        internal static string @Exception_InvokeMiddlewareNoService => GetResourceString("Exception_InvokeMiddlewareNoService");
        /// <summary>Unable to resolve service for type '{0}' while attempting to Invoke middleware '{1}'.</summary>
        internal static string FormatException_InvokeMiddlewareNoService(object p0, object p1)
           => string.Format(Culture, GetResourceString("Exception_InvokeMiddlewareNoService"), p0, p1);

        /// <summary>The '{0}' method must not have ref or out parameters.</summary>
        internal static string @Exception_InvokeDoesNotSupportRefOrOutParams => GetResourceString("Exception_InvokeDoesNotSupportRefOrOutParams");
        /// <summary>The '{0}' method must not have ref or out parameters.</summary>
        internal static string FormatException_InvokeDoesNotSupportRefOrOutParams(object p0)
           => string.Format(Culture, GetResourceString("Exception_InvokeDoesNotSupportRefOrOutParams"), p0);

        /// <summary>The value must be greater than zero.</summary>
        internal static string @Exception_PortMustBeGreaterThanZero => GetResourceString("Exception_PortMustBeGreaterThanZero");
        /// <summary>No service for type '{0}' has been registered.</summary>
        internal static string @Exception_UseMiddlewareNoMiddlewareFactory => GetResourceString("Exception_UseMiddlewareNoMiddlewareFactory");
        /// <summary>No service for type '{0}' has been registered.</summary>
        internal static string FormatException_UseMiddlewareNoMiddlewareFactory(object p0)
           => string.Format(Culture, GetResourceString("Exception_UseMiddlewareNoMiddlewareFactory"), p0);

        /// <summary>'{0}' failed to create middleware of type '{1}'.</summary>
        internal static string @Exception_UseMiddlewareUnableToCreateMiddleware => GetResourceString("Exception_UseMiddlewareUnableToCreateMiddleware");
        /// <summary>'{0}' failed to create middleware of type '{1}'.</summary>
        internal static string FormatException_UseMiddlewareUnableToCreateMiddleware(object p0, object p1)
           => string.Format(Culture, GetResourceString("Exception_UseMiddlewareUnableToCreateMiddleware"), p0, p1);

        /// <summary>Types that implement '{0}' do not support explicit arguments.</summary>
        internal static string @Exception_UseMiddlewareExplicitArgumentsNotSupported => GetResourceString("Exception_UseMiddlewareExplicitArgumentsNotSupported");
        /// <summary>Types that implement '{0}' do not support explicit arguments.</summary>
        internal static string FormatException_UseMiddlewareExplicitArgumentsNotSupported(object p0)
           => string.Format(Culture, GetResourceString("Exception_UseMiddlewareExplicitArgumentsNotSupported"), p0);

        /// <summary>Argument cannot be null or empty.</summary>
        internal static string @ArgumentCannotBeNullOrEmpty => GetResourceString("ArgumentCannotBeNullOrEmpty");
        /// <summary>An element with the key '{0}' already exists in the {1}.</summary>
        internal static string @RouteValueDictionary_DuplicateKey => GetResourceString("RouteValueDictionary_DuplicateKey");
        /// <summary>An element with the key '{0}' already exists in the {1}.</summary>
        internal static string FormatRouteValueDictionary_DuplicateKey(object p0, object p1)
           => string.Format(Culture, GetResourceString("RouteValueDictionary_DuplicateKey"), p0, p1);

        /// <summary>The type '{0}' defines properties '{1}' and '{2}' which differ only by casing. This is not supported by {3} which uses case-insensitive comparisons.</summary>
        internal static string @RouteValueDictionary_DuplicatePropertyName => GetResourceString("RouteValueDictionary_DuplicatePropertyName");
        /// <summary>The type '{0}' defines properties '{1}' and '{2}' which differ only by casing. This is not supported by {3} which uses case-insensitive comparisons.</summary>
        internal static string FormatRouteValueDictionary_DuplicatePropertyName(object p0, object p1, object p2, object p3)
           => string.Format(Culture, GetResourceString("RouteValueDictionary_DuplicatePropertyName"), p0, p1, p2, p3);


    }
}