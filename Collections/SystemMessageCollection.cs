﻿/*
Copyright ©  Olav Christian Botterli.

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/


#region Header

/*
Copyright ©  Olav Christian Botterli. 

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/

#endregion Header

namespace WebGrid.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using WebGrid.Design;
    using WebGrid.Enums;

    /// <summary>
    /// A collection of errors that are generated by <see cref="WebGrid.Grid">WebGrid.Grid</see>.
    /// There are two types of systemMessages; "critical error" and "Error". A "critical error"
    /// is caused by an internal system error and will abort the current evaluation and display a message with details about the systemMessage.
    /// "Error" is displayed in the application to inform the user of a certain event that the user triggered. A
    /// typical example of such an event is when data input from the user is invalid.
    /// </summary>
    public class SystemMessageCollection : List<SystemMessage>
    {
        #region Fields

        private readonly Grid m_Grid;
        private readonly SystemMessageStyle m_SystemMessageStyle;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemMessageCollection">error collection</see> class.
        /// </summary>
        /// <param name="parent">Owner of this object.</param>
        /// <param name="defaultSystemMessageStyle">Determines how the errors should be displayed. This can be overridden for each individual systemMessage.</param>
        public SystemMessageCollection(Grid parent, SystemMessageStyle defaultSystemMessageStyle)
        {
            m_Grid = parent;
            m_SystemMessageStyle = defaultSystemMessageStyle;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Returns how many critical errors there are in the collection.
        /// </summary>
        public int CriticalCount
        {
            get
            {
                if (Count == 0) return 0;
                int numberOfSystemMessages = 0;
                for (int i = 0; i < Count; i++)
                {
                    if (this[i].Critical == false) continue;
                    numberOfSystemMessages++;
                }
                return numberOfSystemMessages;
            }
        }

        internal string RenderSystemMessage
        {
            get
            {
                if (m_Grid.Trace.IsTracing) m_Grid.Trace.Trace("Start SystemMessageCollection.Render");
                if (Count == 0)
                {
                    if (m_Grid.Trace.IsTracing) m_Grid.Trace.Trace("End SystemMessageCollection.Render, no errors..");
                    return null;
                }

                bool isWebGridSystemMessageStyle = false;

                for (int i = 0; i < Count; i++)
                    if (base[i].SystemMessageStyle == SystemMessageStyle.WebGrid)
                    {
                        isWebGridSystemMessageStyle = true;
                        break;
                    }

                if (!isWebGridSystemMessageStyle)
                {
                    if (m_Grid.Trace.IsTracing)
                        m_Grid.Trace.Trace("SystemMessageCollection.Render, no critical errors");
                    return null;
                }
                StringBuilder systemmessage = new StringBuilder();

                if (m_Grid.IsUsingJQueryUICSSFramework)
                    systemmessage.AppendFormat(
                    "<table id=\"wgSystemMessage_{2}\" class=\"ui-state-error wgsystemmessagebox\" style=\"margin-bottom: 5px\" width=\"{0}\"><tr><td class=\"ui-state-error-text wgsystemmessagecell\">{1}</td></tr><tr><td class=\"wgsystemmessagecell\">",
                    m_Grid.Width, m_Grid.GetSystemMessage("SystemMessage"),m_Grid.ID);
                else
                    systemmessage.AppendFormat(
                    "<table id=\"wgSystemMessage_{2}\" class=\"wgsystemmessagebox\" width=\"{0}\" ><tr><td class=\"wgsystemmessagecell\">{1}</td></tr><tr><td class=\"wgsystemmessagecell\">",
                    m_Grid.Width, m_Grid.GetSystemMessage("SystemMessage"), m_Grid.ID);

                ForEach(delegate(SystemMessage message)
                            {
                                systemmessage.AppendFormat("{0}<br/>", message.Message);

                            });
                systemmessage.Append("</td></tr></table>");
               
                if (m_Grid.Trace.IsTracing) m_Grid.Trace.Trace("Finished SystemMessageCollection.Render");
                return systemmessage.ToString();
            }
        }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Returns the error at the specified rowID +";"+columnID.
        /// </summary>
        /// <remarks>
        /// If an error with the specified rowID+";"+columnID does not exist in the table then null is returned. 
        ///</remarks>
        public SystemMessage this[string rowcolumnID]
        {
            get
            {
                for (int i = 0; Count > i; i++)
                {
                    if (String.Compare(base[i].RowColumnID, rowcolumnID, false) == 0)
                        return base[i];
                }
                return null;
            }
        }

        #endregion Indexers

        #region Methods

        /*
        /// <summary>
        /// Adds a new error to the collection.
        /// </summary>
        /// <param name="systemMessage">The error message to be shown.</param>
        /// <param name="critical">Critical errors are errors not caused by users. Critical errors also halt most actions after the error occurred.</param>
        /// <returns>
        /// The index of the new error in the collection.
        /// </returns>
        /// <remarks>errors added at PreRender will not affect update/insert events.</remarks>
        public int Add(string systemMessage, bool critical)
        {
            return AddSystemMessage(systemMessage, critical, m_SystemMessageStyle, null);
        }

        /// <summary>
        /// Adds a new error to the collection.
        /// </summary>
         /// <param name="systemMessage">The error message to be displayed.</param>
        /// <param name="critical">Critical errors are errors not caused by users. Critical errors also halt most actions after the error occurred.</param>
        /// <param name="systemMessageStyle">Determines how the error should be displayed.</param>
        /// <returns>The index of the new error in the collection.</returns>
        public int Add(string systemMessage, bool critical, SystemMessageStyle systemMessageStyle)
        {
            return AddSystemMessage(systemMessage, critical, systemMessageStyle, null);
        }*/
        /// <summary>
        /// Adds a new error to the collection.
        /// </summary>
        /// <param name="systemMessage">The error message to be shown.</param>
        /// <param name="critical">Critical errors are errors not caused by users. Critical errors also halt most actions after the error occurred.</param>
        /// <param name="systemMessageStyle">Determines how the error should be displayed.</param>
        /// <param name="rowColumnId">Determines which row and column the error should be displayed in.</param>
        /// <returns>The index of the new error in the collection.</returns>
        public int Add( string systemMessage, bool critical, SystemMessageStyle systemMessageStyle,
            string rowColumnId)
        {
            return AddSystemMessage(systemMessage, critical, systemMessageStyle, rowColumnId);
        }

        /*
        /// <summary>
        /// Adds a new error to the collection. The error will not be critical.
        /// </summary>
         /// <param name="systemMessage">The error message to be shown.</param>
        /// <param name="systemMessageStyle">Determines how the error should be displayed.</param>
        /// <returns>The index of the new error in the collection.</returns>
        public int Add(string systemMessage, SystemMessageStyle systemMessageStyle)
        {
            return AddSystemMessage(parent, systemMessage, false, systemMessageStyle, null);
        }
        */
        /// <summary>
        /// Adds a new error to the collection. The error will not be critical.
        /// </summary>
        /// <param name="systemMessage">The error message to be shown.</param>
        /// <returns>
        /// The index of the new error in the collection.
        /// </returns>
        /// <remarks>errors added at PreRender will not affect update/insert events.</remarks>
        public int Add(string systemMessage)
        {
            return AddSystemMessage( systemMessage, false, SystemMessageStyle.WebGrid, null);
        }

        /// <summary>
        /// Adds a new error to the collection.
        /// </summary>
        /// <param name="systemMessage">The error message to be shown.</param>
        /// <param name="critical">Critical errors are errors not caused by users. Critical errors also halt most actions after the error occurred.</param>
        /// <returns>
        /// The index of the new error in the collection.
        /// </returns>
        /// <remarks>errors added at PreRender will not affect update/insert events.</remarks>
        public int Add(string systemMessage, bool critical)
        {
            return AddSystemMessage(systemMessage, critical, m_SystemMessageStyle, null);
        }

        /// <summary>
        /// Adds a new error to the collection.
        /// </summary>
        /// <param name="systemMessage">The error message to be shown.</param>
        /// <param name="critical">Critical errors are errors not caused by users. Critical errors also halt most actions after the error occurred.</param>
        /// <param name="systemMessageStyle">Determines how the error should be displayed.</param>
        /// <returns>The index of the new error in the collection.</returns>
        /// <remarks>errors added at PreRender will not affect update/insert events.</remarks>
        public int Add(string systemMessage, bool critical, SystemMessageStyle systemMessageStyle)
        {
            return AddSystemMessage( systemMessage, critical, systemMessageStyle, null);
        }

        /// <summary>
        /// Adds a new error to the collection. The error will not be critical.
        /// </summary>
        /// <param name="systemMessage">The error message to be shown.</param>
        /// <param name="systemMessageStyle">Determines how the error should be displayed.</param>
        /// <returns>The index of the new error in the collection.</returns>
        /// <remarks>errors added at PreRender will not affect update/insert events.</remarks>
        public int Add(string systemMessage, SystemMessageStyle systemMessageStyle)
        {
            return AddSystemMessage(systemMessage, false, systemMessageStyle, null);
        }

        /// <summary>
        /// Adds a new error to the collection. The error will not be critical.
        /// </summary>
        /// <param name="systemMessage">The error message to be shown.</param>
        /// <param name="systemMessageStyle">Determines how the error should be displayed.</param>
        /// <param name="rowColumnId">Determines which row and column the error should be displayed in.</param>
        /// <returns>The index of the new error in the collection.</returns>
        /// <remarks>errors added at PreRender will not affect update/insert events.</remarks>
        public int Add(string systemMessage, SystemMessageStyle systemMessageStyle, string rowColumnId)
        {
            return AddSystemMessage(systemMessage, false, systemMessageStyle, rowColumnId);
        }

        /// <summary>
        /// Adds a new error to the collection. The error will not be critical.
        /// </summary>
        /// <param name="systemMessage">The error message to be shown.</param>
        /// <returns>
        /// The index of the new error in the collection.
        /// </returns>
        /// <remarks>errors added at PreRender will not affect update/insert events.</remarks>
        public int Add(StringBuilder systemMessage)
        {
            return AddSystemMessage(systemMessage.ToString(), false, m_SystemMessageStyle, null);
        }

        /// <summary>
        /// Removes the error at the specified index.
        /// </summary>
        /// <param name="index">The index to be removed.</param>
        public void Remove(int index)
        {
            if (index < Count - 1 && index >= 0)
                RemoveAt(index);
        }

        private int AddSystemMessage(string systemMessage, bool critical,
            SystemMessageStyle systemMessageStyle, string rowColumnId)
        {
            SystemMessage e = new SystemMessage( systemMessage, critical, systemMessageStyle, rowColumnId);

            Add(e);
            return Count;
        }

        #endregion Methods
    }
}