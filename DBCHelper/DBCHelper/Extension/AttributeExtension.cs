

using System;

namespace DBCHelper.Extension
{
    public static class AttributeExtension
    {
        /// <summary>
        /// Attribute 객체에 매개변수에 들어있는 데이터를 삽입합니다.
        /// </summary>
        /// <param name="attribute">복사 되는 Attribute</param>
        /// <param name="copyData">복사에 사용할 Attribute</param>
        public static void CopyAttribute(this AttributeCAN attribute, AttributeCAN copyData)
        {
            if (attribute is null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            if(copyData is null)
            {
                throw new ArgumentNullException(nameof(copyData));
            }
            
            attribute.AttributeName = copyData.AttributeName;
            attribute.AttributeType = copyData.AttributeType;
            attribute.AttributeValueType = copyData.AttributeValueType;
            attribute.Default = copyData.Default;
            attribute.EnumValueList.Clear();

            foreach (var enumValue in copyData.EnumValueList)
            {
                attribute.EnumValueList.Add(enumValue);
            }

            attribute.Maximum = copyData.Maximum;
            attribute.Minimum = copyData.Minimum;
        }

    }
}
