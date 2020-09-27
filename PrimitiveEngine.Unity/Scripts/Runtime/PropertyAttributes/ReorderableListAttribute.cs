namespace PrimitiveEngine.Unity
{
    using System;
    using UnityEngine;

    [AttributeUsage(AttributeTargets.Field)]
    public class ReorderableListAttribute : PropertyAttribute
    {
        public const string SingularPluralBlockBegin = "{{";
        public const string SingularPluralBlockSeparator = "|";
        public const string SingularPluralBlockEnd = "}}";

        public float r, g, b;

        public bool disableAdding;

        public bool disableRemoving;

        public bool disableDragging;

        public bool elementsAreSubassets;

        public string elementHeaderFormat;

        public string listHeaderFormat;

        public bool hideFooterButtons;

        public string[] parallelListNames;

        public Action<int> onRemovingElementCallback;

        public ParallelListLayout parallelListLayout;

        #region Constructors
        public ReorderableListAttribute() { }


        public ReorderableListAttribute(params string[] parallelListNames)
        {
            this.parallelListNames = parallelListNames;
        }
        #endregion


        public enum ParallelListLayout { Rows, Columns };


        #region Properties
        public bool disableAddingAndRemoving
        {
            get { return this.disableAdding && this.disableRemoving; }
            set { this.disableAdding = this.disableRemoving = value; }
        }


        public string PluralListHeaderFormat
        {
            get
            {
                if (this.listHeaderFormat == null)
                    return null;
                string value = this.listHeaderFormat;
                while (value.Contains(SingularPluralBlockBegin)) {
                    int beg = value.IndexOf(SingularPluralBlockBegin);
                    int end = value.IndexOf(SingularPluralBlockEnd, beg);
                    if (end < 0) break;
                    end += SingularPluralBlockEnd.Length;
                    int blockLen = end - beg;
                    string block = value.Substring(beg, blockLen);
                    int sep = value.IndexOf(SingularPluralBlockSeparator, beg);
                    if (sep < 0) {
                        beg += SingularPluralBlockBegin.Length;
                        end -= SingularPluralBlockEnd.Length;
                        int pluralLen = (end - beg);
                        string plural = value.Substring(beg, pluralLen);
                        value = value.Replace(block, plural);
                    }
                    else
                    {
                        sep = sep + SingularPluralBlockSeparator.Length;
                        end -= SingularPluralBlockEnd.Length;
                        int pluralLen = (end - sep);
                        string plural = value.Substring(beg, pluralLen);
                        value = value.Replace(block, plural);
                    }
                }
                return value;
            }
        }


        public string SingularListHeaderFormat
        {
            get
            {
                if (this.listHeaderFormat == null)
                    return null;
                string value = this.listHeaderFormat;
                while (value.Contains(SingularPluralBlockBegin)) {
                    int beg = value.IndexOf(SingularPluralBlockBegin);
                    int end = value.IndexOf(SingularPluralBlockEnd, beg);
                    if (end < 0) break;
                    end += SingularPluralBlockEnd.Length;
                    int blockLen = end - beg;
                    string block = value.Substring(beg, blockLen);
                    int sep = value.IndexOf(SingularPluralBlockSeparator, beg);
                    if (sep < 0) {
                        value = value.Replace(block, "");
                    }
                    else
                    {
                        beg += SingularPluralBlockBegin.Length;
                        int singularLen = (sep - beg);
                        string singular = value.Substring(beg, singularLen);
                        value = value.Replace(block, singular);
                    }
                }
                return value;
            }
        }
        #endregion
    }

}