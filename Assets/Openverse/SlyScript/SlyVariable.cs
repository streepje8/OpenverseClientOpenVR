using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sly
{
    public enum SlyObjectType
    {
        TypeString,
        Typeint,
        Typefloat,
        Typedouble,
        Typereference,
        TypeSlyObject,
        Typevoid,
        TypeUndefined,
    }

    [Serializable]
    public class SlyVariable
    {
        public string name = "undefined";
        public SlyObjectType type = SlyObjectType.TypeUndefined;
        public string value = null;
    }
}

