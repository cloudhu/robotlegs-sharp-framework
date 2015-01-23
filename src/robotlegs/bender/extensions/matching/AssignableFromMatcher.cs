//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using robotlegs.bender.framework.api;


namespace robotlegs.bender.extensions.matching
{
	public class AssignableFromMatcher : IMatcher
	{
		private Type _type;
		
		public AssignableFromMatcher(Type type)
		{
			_type = type;
		}
		
		public bool Matches(object item)
		{
			Type t1 = _type;
			Type i1 = item.GetType ();
			bool b1 = t1.IsAssignableFrom (i1);
			/*
			IContextView IsAssignableFrom( Contextview ) == true;
			ContextView IsAssignableFrom( IContextview ) == false;
			//*/
			
			Type t2 = item.GetType ();
			bool b2 = t2.IsAssignableFrom (_type);
			
			
			return b1;
			return item.GetType().IsAssignableFrom(_type);
		}
	}
}
