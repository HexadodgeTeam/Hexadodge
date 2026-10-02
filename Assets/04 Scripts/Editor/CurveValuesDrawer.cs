using UnityEditor;
using UnityEngine;

public abstract class CurveValueDrawerBase : PropertyDrawer
{
    private const string InnerField = "curve";

    private static readonly string[] FloatFieldNames =
    {
        "m_ConstantMin", "m_ConstantMax"
    };

    protected abstract bool WholeNumbers { get; }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(InnerField), label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty inner = property.FindPropertyRelative(InnerField);

        if (label != null && label.text == property.name)
            label = new GUIContent(ObjectNames.NicifyVariableName(property.name), label.tooltip);

        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(position, inner, label);
        
        if (EditorGUI.EndChangeCheck() && WholeNumbers && !inner.hasMultipleDifferentValues)
        {
            foreach (string fieldName in FloatFieldNames)
            {
                SerializedProperty field = inner.FindPropertyRelative(fieldName);
                
                if (field != null && field.propertyType == SerializedPropertyType.Float)
                    field.floatValue = Mathf.Round(field.floatValue);
            }
        }
    }
}

[CustomPropertyDrawer(typeof(FloatCurve))]
public class FloatCurveDrawer : CurveValueDrawerBase
{
    protected override bool WholeNumbers => false;
}

[CustomPropertyDrawer(typeof(IntCurve))]
public class IntCurveDrawer : CurveValueDrawerBase
{
    protected override bool WholeNumbers => true;
}