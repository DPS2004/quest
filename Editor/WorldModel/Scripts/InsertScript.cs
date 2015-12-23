﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextAdventures.Quest.Functions;

namespace TextAdventures.Quest.Scripts
{
    public class InsertScriptConstructor : ScriptConstructorBase
    {
        public override string Keyword
        {
            get { return "insert"; }
        }

        protected override IScript CreateInt(List<string> parameters, ScriptContext scriptContext)
        {
            return new InsertScript(scriptContext, new Expression<string>(parameters[0], scriptContext));
        }

        protected override int[] ExpectedParameters
        {
            get { return new int[] { 1 }; }
        }
    }

    public class InsertScript : ScriptBase
    {
        private ScriptContext m_scriptContext;
        private WorldModel m_worldModel;
        private IFunction<string> m_filename;

        public InsertScript(ScriptContext scriptContext, IFunction<string> filename)
        {
            m_scriptContext = scriptContext;
            m_worldModel = scriptContext.WorldModel;
            m_filename = filename;
        }

        protected override ScriptBase CloneScript()
        {
            return new InsertScript(m_scriptContext, m_filename.Clone());
        }

        public override string Save()
        {
            return SaveScript("insert", m_filename.Save());
        }

        public override object GetParameter(int index)
        {
            return m_filename.Save();
        }

        public override void SetParameterInternal(int index, object value)
        {
            m_filename = new Expression<string>((string)value, m_scriptContext);
        }

        public override string Keyword
        {
            get
            {
                return "insert";
            }
        }
    }
}
