//------------------------------------------------------------------------------
//  Copyright (c) 2014-2016 the original author or authors. All Rights Reserved. 
// 
//  NOTICE: You are permitted to use, modify, and distribute this file 
//  in accordance with the terms of the license agreement accompanying it. 
//------------------------------------------------------------------------------

﻿using System;
using System.Collections.Generic;
using Robotlegs.Bender.Extensions.CommandCenter.API;
using Robotlegs.Bender.Extensions.CommandCenter.Impl;
using Robotlegs.Bender.Extensions.EventManagement.API;
using Robotlegs.Bender.Extensions.EventManagement.Impl;
using Robotlegs.Bender.Framework.API;

namespace Robotlegs.Bender.Extensions.EventCommand.Impl
{
	public class EventCommandTrigger : ICommandTrigger
	{
		/*============================================================================*/
		/* Private Properties                                                         */
		/*============================================================================*/

		private IEventDispatcher _dispatcher;

		private Enum _type;

		private Type _eventClass;

		private ICommandMappingList _mappings;

		private ICommandExecutor _executor;

		/*============================================================================*/
		/* Constructor                                                                */
		/*============================================================================*/

		public EventCommandTrigger (IInjector injector, IEventDispatcher dispatcher, Enum type, Type eventClass = null, IEnumerable<CommandMappingList.Processor> processors = null, ILogging logger = null)
		{
			_dispatcher = dispatcher;
			_type = type;
			_eventClass = eventClass;
			_mappings = new CommandMappingList(this, processors, logger);
			_executor = new CommandExecutor(injector, _mappings.RemoveMapping);
		}

		/*============================================================================*/
		/* Public Functions                                                           */
		/*============================================================================*/

		public CommandMapper CreateMapper()
		{
			return new CommandMapper(_mappings);
		}

		public void Activate ()
		{
			_dispatcher.AddEventListener(_type, (Action<IEvent>) EventHandler);
		}

		public void Deactivate ()
		{
			_dispatcher.RemoveEventListener (_type, (Action<IEvent>) EventHandler);
		}

		public override string ToString ()
		{
			return _eventClass + " with selector '" + _type.ToString() + "'";
		}

		/*============================================================================*/
		/* Private Functions                                                          */
		/*============================================================================*/

		private void EventHandler(IEvent evt)
		{
			/**
			 * Map(CustomType, typeof(IEvent)). 	Dispatch(new CustomEvent(CustomType)).	Make it inject IEvent
			 * Map(CustomType). 					Dispatch(new Event(CustomType)).		Make it inject IEvent
			 * Map(CustomType). 					Dispatch(new CustomEvent(CustomType)).	Make it inject CustomEvent
			 * 
			 * Map(CustomType).typeof(CustomType)	Dispatch(new Event(CustomType)).		Make it not execute
			 */

			Type evtType = evt.GetType ();
			Type payloadEvtType = null;
			if (evtType == _eventClass || (_eventClass == null))
			{
				payloadEvtType = (evtType == typeof(Event)) ? typeof(IEvent) : evtType;
			}
			else if (_eventClass == typeof(IEvent))
			{
				payloadEvtType = _eventClass;
				payloadEvtType = typeof (IEvent);
			}
			else
				return;

			_executor.ExecuteCommands(_mappings.GetList(), new CommandPayload (new List<object>{ evt }, new List<Type>{ payloadEvtType }));
		}
	}
}

