//This is generated code. Do not modify this file; modify the template that produces it.

using System;

namespace RMUD
{
	public class RuleDelegateWrapper<TR>
	{
		public virtual TR Invoke(Object[] Arguments)
		{
			throw new NotImplementedException();
		}		
		
		public static RuleDelegateWrapper<TR> MakeWrapper<T0>(Func<T0, TR> Delegate)
		{
			return new RuleDelegateWrapper<T0, TR> { Delegate = Delegate };
		}		
		
		public static RuleDelegateWrapper<TR> MakeWrapper<T0, T1>(Func<T0, T1, TR> Delegate)
		{
			return new RuleDelegateWrapper<T0, T1, TR> { Delegate = Delegate };
		}		
		
		public static RuleDelegateWrapper<TR> MakeWrapper<T0, T1, T2>(Func<T0, T1, T2, TR> Delegate)
		{
			return new RuleDelegateWrapper<T0, T1, T2, TR> { Delegate = Delegate };
		}		
		
		public static RuleDelegateWrapper<TR> MakeWrapper<T0, T1, T2, T3>(Func<T0, T1, T2, T3, TR> Delegate)
		{
			return new RuleDelegateWrapper<T0, T1, T2, T3, TR> { Delegate = Delegate };
		}		
	}
	
	public class RuleDelegateWrapper<T0, TR> : RuleDelegateWrapper<TR>
	{
		internal Func<T0, TR> Delegate;

		public override TR Invoke(Object[] Arguments)
		{
			return (TR)Delegate.DynamicInvoke(Arguments);
		}
	}
	
	public class RuleDelegateWrapper<T0, T1, TR> : RuleDelegateWrapper<TR>
	{
		internal Func<T0, T1, TR> Delegate;

		public override TR Invoke(Object[] Arguments)
		{
			return (TR)Delegate.DynamicInvoke(Arguments);
		}
	}
	
	public class RuleDelegateWrapper<T0, T1, T2, TR> : RuleDelegateWrapper<TR>
	{
		internal Func<T0, T1, T2, TR> Delegate;

		public override TR Invoke(Object[] Arguments)
		{
			return (TR)Delegate.DynamicInvoke(Arguments);
		}
	}
	
	public class RuleDelegateWrapper<T0, T1, T2, T3, TR> : RuleDelegateWrapper<TR>
	{
		internal Func<T0, T1, T2, T3, TR> Delegate;

		public override TR Invoke(Object[] Arguments)
		{
			return (TR)Delegate.DynamicInvoke(Arguments);
		}
	}

}

