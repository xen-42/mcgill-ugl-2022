using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public abstract class BaseCondition
{
    public abstract bool IsValid();
}

[System.Serializable]
public abstract class BaseBlackboardCondition : BaseCondition, IComponentReveiver<BlackboardManager>
{
    protected BlackboardManager m_manager;

    public BlackboardManager AddComponent(BlackboardManager component)
    {
        m_manager = component;
        return m_manager;
    }
}

[System.Serializable]
public class IntBlackboardCondition : BaseBlackboardCondition
{
    public enum IntegerComparison
    { Equals, NotEqual, Greater, Less };

    public string valueName;

    public IntegerComparison type;

    public int valueAgainst;

    public override bool IsValid()
    {
        int value = m_manager.GetInteger(valueName);
        return type switch
        {
            IntegerComparison.Equals => value == valueAgainst,
            IntegerComparison.NotEqual => value != valueAgainst,
            IntegerComparison.Greater => value > valueAgainst,
            IntegerComparison.Less => value < valueAgainst,
            _ => throw new System.NotImplementedException(),
        };
    }
}

[System.Serializable]
public class FloatBlackboardCondition : BaseBlackboardCondition
{
    public enum FloatComparison
    { Less, Greater }

    public string valueName;
    public FloatComparison type;
    public float valueAgainst;

    public override bool IsValid() => (type == FloatComparison.Less) ? m_manager.GetFloat(valueName) < valueAgainst : m_manager.GetFloat(valueName) > valueAgainst;
}

[System.Serializable]
public class BoolBlackboardCondition : BaseBlackboardCondition
{
    public enum BoolComparison
    { True, False }

    public string valueName;
    public BoolComparison type;

    public override bool IsValid() => (type == BoolComparison.True) ? m_manager.GetBool(valueName) : !m_manager.GetBool(valueName);
}

[System.Serializable]
public class TriggerBlackboardCondition : BaseBlackboardCondition
{
    public string valueName;

    public override bool IsValid()
    {
        bool res;
        if (res = m_manager.GetTrigger(valueName))
            m_manager.ResetTrigger(valueName);

        return res;
    }
}

[System.Serializable]
public class AndCondition : BaseCondition
{
    public List<BaseCondition> conditions = new List<BaseCondition>();

    public override bool IsValid() => conditions.All(cond => cond.IsValid());
}

[System.Serializable]
public class OrCondition : BaseCondition
{
    public List<BaseCondition> conditions = new List<BaseCondition>();

    public override bool IsValid() => conditions.Any(cond => cond.IsValid());
}