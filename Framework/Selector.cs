using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NetworkExtensions.Framework
{
    public interface IMemberSelector
    {
        string MemberName { get; }
    }

    public class MemberSelector<T, TProperty> : IMemberSelector
    {
        private Expression<Func<T, TProperty>> _memberSelector;
        private string _memberName;

        public MemberSelector(Expression<Func<T, TProperty>> memberSelector)
        {
            _memberSelector = memberSelector;
            _memberName = memberSelector.GetSelectedMemberName();
        }

        public string MemberName
        {
            get { throw new NotImplementedException(); }
        }
    }

    public static class Selector
    {
        public static IMemberSelector NetInfo<TProperty>(Expression<Func<NetInfo, TProperty>> propertySelector)
        {
            return new MemberSelector<NetInfo, TProperty>(propertySelector);
        }

        public static IMemberSelector NetAI<TProperty>(Expression<Func<NetAI, TProperty>> propertySelector)
        {
            return new MemberSelector<NetAI, TProperty>(propertySelector);
        }

        public static IMemberSelector RoadAI<TProperty>(Expression<Func<RoadAI, TProperty>> propertySelector)
        {
            return new MemberSelector<RoadAI, TProperty>(propertySelector);
        }
    }
}
