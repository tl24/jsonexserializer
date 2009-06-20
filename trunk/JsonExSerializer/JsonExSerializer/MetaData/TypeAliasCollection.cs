using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework;
using System.Reflection;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Provides a collection of bindings from Type to alias.  When type information is rendered for any types
    /// registered in the collection, its alias is rendered instead.
    /// </summary>
    public class TypeAliasCollection
    {
        protected TwoWayDictionary<Type, string> bindings;
        protected List<Assembly> assemblyList;
        /// <summary>
        /// Initializes a default intance of the TypeAliasCollection with aliases for primitive types
        /// </summary>
        public TypeAliasCollection()
        {
            this.bindings = new TwoWayDictionary<Type, string>();
            this.assemblyList = new List<Assembly>();
            AddDefaultAliases();
            AddDefaultAssemblies();
        }

        protected virtual void AddDefaultAssemblies()
        {
            Assemblies.Add(typeof(int).Assembly);
        }

        /// <summary>
        /// Adds default aliases for primitive types
        /// </summary>
        protected virtual void AddDefaultAliases()
        {
            // add bindings for primitive types
            bindings[typeof(byte)] = "byte";
            bindings[typeof(sbyte)] = "sbyte";
            bindings[typeof(bool)] = "bool";
            bindings[typeof(short)] = "short";
            bindings[typeof(ushort)] = "ushort";
            bindings[typeof(int)] = "int";
            bindings[typeof(uint)] = "uint";
            bindings[typeof(long)] = "long";
            bindings[typeof(ulong)] = "ulong";
            bindings[typeof(string)] = "string";
            bindings[typeof(object)] = "object";
            bindings[typeof(char)] = "char";
            bindings[typeof(decimal)] = "decimal";
            bindings[typeof(float)] = "float";
            bindings[typeof(double)] = "double";
        }

        /// <summary>
        /// Adds an alias for a type.  When the type is encountered during serialization, the alias
        /// will be written out instead of the normal type info if a cast or type information is needed.
        /// </summary>
        /// <param name="type">the type object</param>
        /// <param name="alias">the type's alias</param>
        public virtual void Add(Type type, string alias)
        {
            bindings[type] = alias;
        }

        /// <summary>
        /// Clears all aliases
        /// </summary>
        public virtual void Clear()
        {
            bindings.Clear();
        }

        /// <summary>
        /// Removes an alias for a specific type
        /// </summary>
        /// <param name="type">the type to remove</param>
        public virtual void Remove(Type type)
        {
            bindings.Remove(type);
        }

        /// <summary>
        /// Removes an alias for a specific type by alias
        /// </summary>
        /// <param name="alias">the alias to remove</param>
        public virtual void Remove(string alias)
        {
            Type key;
            if (bindings.TryGetKey(alias, out key))
                bindings.Remove(key);

        }

        /// <summary>
        /// Looks up an alias for a registered type
        /// </summary>
        /// <param name="type">the type to lookup</param>
        /// <returns>a type alias or null if not found</returns>
        public virtual string GetTypeAlias(Type type)
        {
            string alias = null;
            if (!bindings.TryGetValue(type, out alias))
            {
                alias = null;
            }
            return alias;
        }

        /// <summary>
        /// Looks up an alias for a registered type
        /// </summary>
        /// <param name="type">the type to lookup</param>
        /// <returns>a type alias or null if not found</returns>
        public string this[Type type]
        {
            get { return GetTypeAlias(type); }
        }

        /// <summary>
        /// Looks up a type, given an alias for the type.
        /// </summary>
        /// <param name="alias">the alias to look up</param>
        /// <returns>the type representing the alias or null</returns>
        public virtual Type GetTypeBinding(string alias)
        {
            Type t = null;
            if (!bindings.TryGetKey(alias, out t))
            {
                t = null;
            }
            return t;
        }

        /// <summary>
        /// Looks up a type, given an alias for the type.
        /// </summary>
        /// <param name="typeAlias">the alias to look up</param>
        /// <returns>the type representing the alias or null</returns>
        public Type this[string alias]
        {
            get { return GetTypeBinding(alias); }
        }

        public virtual IList<Assembly> Assemblies
        {
            get { return this.assemblyList; }
        }

        public virtual bool ShouldWriteAssembly(Assembly assembly)
        {
            return !Assemblies.Contains(assembly);
        }
    }
}
