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

		public static RuleDelegateWrapper<TR> MakeWrapper(Func<TR> Delegate)
		{
			return new RuleDelegateWrapperImpl<TR> { Delegate = Delegate };
		}

		public virtual bool AreArgumentsCompatible(Object[] Arguments)
		{
			throw new NotImplementedException();
		}

		
		public static RuleDelegateWrapper<TR> MakeWrapper<T0>(Func<T0, TR> Delegate)
		{
			return new RuleDelegateWrapperImpl<T0, TR> { Delegate = Delegate };
		}		
		
		public static RuleDelegateWrapper<TR> MakeWrapper<T0, T1>(Func<T0, T1, TR> Delegate)
		{
			return new RuleDelegateWrapperImpl<T0, T1, TR> { Delegate = Delegate };
		}		
		
		public static RuleDelegateWrapper<TR> MakeWrapper<T0, T1, T2>(Func<T0, T1, T2, TR> Delegate)
		{
			return new RuleDelegateWrapperImpl<T0, T1, T2, TR> { Delegate = Delegate };
		}		
		
		public static RuleDelegateWrapper<TR> MakeWrapper<T0, T1, T2, T3>(Func<T0, T1, T2, T3, TR> Delegate)
		{
			return new RuleDelegateWrapperImpl<T0, T1, T2, T3, TR> { Delegate = Delegate };
		}		
	}

	public class RuleDelegateWrapperImpl<TR> : RuleDelegateWrapper<TR>
	{
		internal Func<TR> Delegate;

		public override TR Invoke(Object[] Arguments)
		{
			return (TR)Delegate.DynamicInvoke(Arguments);
		}

		public override bool AreArgumentsCompatible(Object[] Arguments)
		{
			return Arguments.Length == 0;
		}
	}

	
	public class RuleDelegateWrapperImpl<T0, TR> : RuleDelegateWrapper<TR>
	{
		internal Func<T0, TR> Delegate;

		public override TR Invoke(Object[] Arguments)
		{
			return (TR)Delegate.DynamicInvoke(Arguments);
		}

		public override bool AreArgumentsCompatible(Object[] Arguments)
		{
			if (Arguments.Length != 1) return false;

			if (Arguments[0] != null && !typeof(T0).IsAssignableFrom(Arguments[0].GetType())) return false;

			return true;
		}
	}
	
	public class RuleDelegateWrapperImpl<T0, T1, TR> : RuleDelegateWrapper<TR>
	{
		internal Func<T0, T1, TR> Delegate;

		public override TR Invoke(Object[] Arguments)
		{
			return (TR)Delegate.DynamicInvoke(Arguments);
		}

		public override bool AreArgumentsCompatible(Object[] Arguments)
		{
			if (Arguments.Length != 2) return false;

			if (Arguments[0] != null && !typeof(T0).IsAssignableFrom(Arguments[0].GetType())) return false;
			if (Arguments[1] != null && !typeof(T1).IsAssignableFrom(Arguments[1].GetType())) return false;

			return true;
		}
	}
	
	public class RuleDelegateWrapperImpl<T0, T1, T2, TR> : RuleDelegateWrapper<TR>
	{
		internal Func<T0, T1, T2, TR> Delegate;

		public override TR Invoke(Object[] Arguments)
		{
			return (TR)Delegate.DynamicInvoke(Arguments);
		}

		public override bool AreArgumentsCompatible(Object[] Arguments)
		{
			if (Arguments.Length != 3) return false;

			if (Arguments[0] != null && !typeof(T0).IsAssignableFrom(Arguments[0].GetType())) return false;
			if (Arguments[1] != null && !typeof(T1).IsAssignableFrom(Arguments[1].GetType())) return false;
			if (Arguments[2] != null && !typeof(T2).IsAssignableFrom(Arguments[2].GetType())) return false;

			return true;
		}
	}
	
	public class RuleDelegateWrapperImpl<T0, T1, T2, T3, TR> : RuleDelegateWrapper<TR>
	{
		internal Func<T0, T1, T2, T3, TR> Delegate;

		public override TR Invoke(Object[] Arguments)
		{
			return (TR)Delegate.DynamicInvoke(Arguments);
		}

		public override bool AreArgumentsCompatible(Object[] Arguments)
		{
			if (Arguments.Length != 4) return false;

			if (Arguments[0] != null && !typeof(T0).IsAssignableFrom(Arguments[0].GetType())) return false;
			if (Arguments[1] != null && !typeof(T1).IsAssignableFrom(Arguments[1].GetType())) return false;
			if (Arguments[2] != null && !typeof(T2).IsAssignableFrom(Arguments[2].GetType())) return false;
			if (Arguments[3] != null && !typeof(T3).IsAssignableFrom(Arguments[3].GetType())) return false;

			return true;
		}
	}

}
