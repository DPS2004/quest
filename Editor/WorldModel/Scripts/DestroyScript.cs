﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextAdventures.Quest.Functions;

namespace TextAdventures.Quest.Scripts
{
    public class DestroyScriptConstructor : ScriptConstructorBase
    {
        public override string Keyword
        {
            get { return "destroy"; }
        }

        protected override IScript CreateInt(List<string> parameters, ScriptContext scriptContext)
        {
            return new DestroyScript(scriptContext, new Expression<string>(parameters[0], scriptContext));
        }

        protected override int[] ExpectedParameters
        {
            get { return new int[] { 1 }; }
        }
    }

    public class DestroyScript : ScriptBase
    {
        private ScriptContext m_scriptContext;
        private WorldModel m_worldModel;
        private IFunction<string> m_expr;

        public DestroyScript(ScriptContext scriptContext, IFunction<string> expr)
        {
            m_scriptContext = scriptContext;
            m_worldModel = scriptContext.WorldModel;
            m_expr = expr;
        }

        protected override ScriptBase CloneScript()
        {
            return new DestroyScript(m_scriptContext, m_expr.Clone());
        }

        public override string Save()
        {
            return SaveScript("destroy", m_expr.Save());
        }

        public override string Keyword
        {
            get
            {
                return "destroy";
            }
        }

        public override object GetParameter(int index)
        {
            return m_expr.Save();
        }

        public override void SetParameterInternal(int index, object value)
        {
            m_expr = new Expression<string>((string)value, m_scriptContext);
        }
    }
}
