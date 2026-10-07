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

    protected virtual string Suffix => "";
    protected virtual Color Tint => Color.clear;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(InnerField), label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty inner = property.FindPropertyRelative(InnerField);

        if (label != null && label.text == property.name)
            label = new GUIContent(ObjectNames.NicifyVariableName(property.name), label.tooltip);

        if (label != null && !string.IsNullOrEmpty(Suffix) && !string.IsNullOrEmpty(label.text))
            label = new GUIContent(label.text + Suffix, label.tooltip);

        if (Tint.a > 0f)
            EditorGUI.DrawRect(position, Tint);

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
    protected override string Suffix => " (float)";
    protected override Color Tint => new(0.3f, 0.6f, 1f, 0.11f);
}

[CustomPropertyDrawer(typeof(IntCurve))]
public class IntCurveDrawer : CurveValueDrawerBase
{
    protected override bool WholeNumbers => true;
    protected override string Suffix => " (int)";
    protected override Color Tint => new(1f, 0.7f, 0.0f, 0.08f);
}
