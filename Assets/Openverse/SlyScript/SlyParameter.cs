using System;

namespace Sly
{
    [Serializable]
    public class SlyParameter
    {
        public string name;
        public string value;
        public bool isVariable = false;
        //IF isVariable == false
        public SlyObjectType type;
        public SlyParameter(SlyVariable var, string value)
        {
            name = var.name;
            type = var.type;
            this.value = value;
        }

        public SlyParameter(SlyObjectType type, string name, string value)
        {
            this.name = name;
            this.value = value;
            this.type = type;
        }
    }
}