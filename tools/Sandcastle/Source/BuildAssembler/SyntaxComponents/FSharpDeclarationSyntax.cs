﻿// Copyright © Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Xml.XPath;


namespace Microsoft.Ddue.Tools
{

    public class FSharpDeclarationSyntaxGenerator : SyntaxGeneratorTemplate
    {

        public FSharpDeclarationSyntaxGenerator(XPathNavigator configuration)
            : base(configuration)
        {
            if (String.IsNullOrEmpty(Language)) Language = "FSharp";
        }

        // namespace: done
        public override void WriteNamespaceSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {

            string name = reflection.Evaluate(apiNameExpression).ToString();

            writer.WriteKeyword("namespace");
            writer.WriteString(" ");
            writer.WriteIdentifier(name);
        }

        public override void WriteClassSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {

            WriteDotNetObject(reflection, writer, "class");
        }

        // TODO: Use apiContainingTypeSubgroupExpression instead of passing in class, struct, interface
        public override void WriteStructureSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            WriteDotNetObject(reflection, writer, "struct");
        }

        public override void WriteInterfaceSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            WriteDotNetObject(reflection, writer, "interface");
        }

        
        public override void WriteDelegateSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {

            string name = (string)reflection.Evaluate(apiNameExpression);
            bool isSerializable = (bool)reflection.Evaluate(apiIsSerializableTypeExpression);

            if (isSerializable) WriteAttribute("T:System.SerializableAttribute", writer);

            WriteAttributes(reflection, writer);

            writer.WriteKeyword("type");
            writer.WriteString(" ");
            writer.WriteIdentifier(name);
            writer.WriteString(" = ");
            writer.WriteLine();
            writer.WriteString("    ");
            writer.WriteKeyword("delegate");
            writer.WriteString(" ");
            writer.WriteKeyword("of");
            writer.WriteString(" ");

            WriteParameters(reflection, writer);

            writer.WriteKeyword("->");
            writer.WriteString(" ");
            WriteReturnValue(reflection, writer);

        }

        public override void WriteEnumerationSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {

            string name = (string)reflection.Evaluate(apiNameExpression);
            bool isSerializable = (bool)reflection.Evaluate(apiIsSerializableTypeExpression);

            if (isSerializable) WriteAttribute("T:System.SerializableAttribute", writer);
            WriteAttributes(reflection, writer);
            writer.WriteKeyword("type");
            writer.WriteString(" ");
            WriteVisibility(reflection, writer);
            writer.WriteIdentifier(name);
        }

        public override void WriteConstructorSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {

            string name = (string)reflection.Evaluate(apiContainingTypeNameExpression);
            bool isStatic = (bool)reflection.Evaluate(apiIsStaticExpression);

            WriteAttributes(reflection, writer);

            writer.WriteKeyword("new");
            writer.WriteString(" : ");
            WriteParameters(reflection, writer);
            writer.WriteKeyword("->");
            writer.WriteString(" ");
            writer.WriteIdentifier(name);

        }

        public override void WriteNormalMethodSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {

            string name = (string)reflection.Evaluate(apiNameExpression);
            bool isOverride = (bool)reflection.Evaluate(apiIsOverrideExpression);
            bool isStatic = (bool)reflection.Evaluate(apiIsStaticExpression);
            bool isVirtual = (bool)reflection.Evaluate(apiIsVirtualExpression) && !(bool)reflection.Evaluate(apiIsAbstractProcedureExpression);
            int iterations = isVirtual ? 2 : 1;

            for (int i = 0; i < iterations; i++)
            {

                WriteAttributes(reflection, writer);

                WriteVisibility(reflection, writer);

                if (isStatic)
                {
                    writer.WriteKeyword("static");
                    writer.WriteString(" ");
                }

                if (isVirtual)
                    if (i == 0)
                    {
                        writer.WriteKeyword("abstract");
                        writer.WriteString(" ");
                    }
                    else
                    {
                        writer.WriteKeyword("override");
                        writer.WriteString(" ");
                    }
                else
                {
                    WriteMemberKeyword(reflection, writer);
                }
 
                writer.WriteIdentifier(name);
                writer.WriteString(" : ");
                WriteParameters(reflection, writer);
                writer.WriteKeyword("->");
                writer.WriteString(" ");
                WriteReturnValue(reflection, writer);
                writer.WriteString(" ");
                WriteGenericTemplateConstraints(reflection, writer);

                if (i == 0)
                    writer.WriteLine();
            }
        }

        public override void WriteOperatorSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            string name = (string)reflection.Evaluate(apiNameExpression);
            string identifier;

            bool isStatic = (bool)reflection.Evaluate(apiIsStaticExpression);

            switch (name)
            {
                // unary math operators
                case "UnaryPlus":
                    identifier = "+";
                    break;
                case "UnaryNegation":
                    identifier = "-";
                    break;
                case "Increment":
                    identifier = "++";
                    break;
                case "Decrement":
                    identifier = "--";
                    break;
                // unary logical operators
                case "LogicalNot":
                    identifier = "not";
                    break;
                case "True":
                    identifier = "true";
                    break;
                case "False":
                    identifier = "false";
                    break;
                // binary comparison operators
                case "Equality":
                    identifier = "=";
                    break;
                case "Inequality":
                    identifier = "<>";
                    break;
                case "LessThan":
                    identifier = "<";
                    break;
                case "GreaterThan":
                    identifier = ">";
                    break;
                case "LessThanOrEqual":
                    identifier = "<=";
                    break;
                case "GreaterThanOrEqual":
                    identifier = ">=";
                    break;
                // binary math operators
                case "Addition":
                    identifier = "+";
                    break;
                case "Subtraction":
                    identifier = "-";
                    break;
                case "Multiply":
                    identifier = "*";
                    break;
                case "Division":
                    identifier = "/";
                    break;
                case "Modulus":
                    identifier = "%";
                    break;
                // binary logical operators
                case "BitwiseAnd":
                    identifier = "&&&";
                    break;
                case "BitwiseOr":
                    identifier = "|||";
                    break;
                case "ExclusiveOr":
                    identifier = "^^^";
                    break;
                // bit-array operators
                case "OnesComplement":
                   identifier = null; // No F# equiv.
                   break;
                case "LeftShift":
                    identifier = "<<<";
                    break;
                case "RightShift":
                    identifier = ">>>";
                    break;
                // unrecognized operator
                default:
                    identifier = null;
                    break;
            }
            if (identifier == null)
            {
                writer.WriteMessage("UnsupportedOperator_" + Language);
            }
            else
            {
                if (isStatic)
                {
                    writer.WriteKeyword("static");
                    writer.WriteString(" ");
                }

                writer.WriteKeyword("let");
                writer.WriteString(" ");
                writer.WriteKeyword("inline");
                writer.WriteKeyword(" ");

                writer.WriteString("(");
                writer.WriteIdentifier(identifier);
                writer.WriteString(")");

                WriteParameters(reflection, writer);
                writer.WriteString(" : ");
                WriteReturnValue(reflection, writer);

            }
        }

        public override void WriteCastSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            writer.WriteMessage("UnsupportedCast_" + Language);
        }

        // DONE
        public override void WritePropertySyntax(XPathNavigator reflection, SyntaxWriter writer)
        {

            string name = (string)reflection.Evaluate(apiNameExpression);
            bool isGettable = (bool)reflection.Evaluate(apiIsReadPropertyExpression);
            bool isSettable = (bool)reflection.Evaluate(apiIsWritePropertyExpression);
            
            bool isStatic = (bool)reflection.Evaluate(apiIsStaticExpression);
            bool isVirtual = (bool)reflection.Evaluate(apiIsVirtualExpression) && !(bool)reflection.Evaluate(apiIsAbstractProcedureExpression);
            int iterations = isVirtual ? 2 : 1;

            for (int i = 0; i < iterations; i++)
            {
                WriteAttributes(reflection, writer);
                WriteVisibility(reflection, writer);

                if (isStatic)
                {
                    writer.WriteKeyword("static");
                    writer.WriteString(" ");
                }

                if (isVirtual)
                    if (i == 0)
                    {
                        writer.WriteKeyword("abstract");
                        writer.WriteString(" ");
                    }
                    else
                    {
                        writer.WriteKeyword("override");
                        writer.WriteString(" ");
                    }
                else
                {
                    WriteMemberKeyword(reflection, writer);
                }

                writer.WriteIdentifier(name);
                writer.WriteString(" : ");
                WriteReturnValue(reflection, writer);

                if (isSettable)
                {
                    writer.WriteString(" ");
                    writer.WriteKeyword("with");
                    writer.WriteString(" ");

                    string getVisibility = (string)reflection.Evaluate(apiGetVisibilityExpression);
                    if (!String.IsNullOrEmpty(getVisibility))
                    {
                        WriteVisibility(getVisibility, writer);
                    }

                    writer.WriteKeyword("get");
                    writer.WriteString(", ");

                    string setVisibility = (string)reflection.Evaluate(apiSetVisibilityExpression);
                    if (!String.IsNullOrEmpty(setVisibility))
                    {
                        WriteVisibility(setVisibility, writer);
                    }

                    writer.WriteKeyword("set");
                }
                if (i == 0)
                    writer.WriteLine();
            }
        }

        public override void WriteEventSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            string name = (string)reflection.Evaluate(apiNameExpression);
            XPathNavigator handler = reflection.SelectSingleNode(apiHandlerOfEventExpression);
            XPathNavigator args = reflection.SelectSingleNode(apiEventArgsExpression);
            bool isVirtual = (bool)reflection.Evaluate(apiIsVirtualExpression) && !(bool)reflection.Evaluate(apiIsAbstractProcedureExpression);
            int iterations = isVirtual ? 2 : 1;

            for (int i = 0; i < iterations; i++)
            {
                WriteAttributes(reflection, writer);
                WriteVisibility(reflection, writer);
                if (isVirtual)
                    if (i == 0)
                    {
                        writer.WriteKeyword("abstract");
                        writer.WriteString(" ");
                    }
                    else
                    {
                        writer.WriteKeyword("override");
                        writer.WriteString(" ");
                    }
                else
                {
                    WriteMemberKeyword(reflection, writer);
                }
                writer.WriteIdentifier(name);
                writer.WriteString(" : ");
                writer.WriteReferenceLink("T:Microsoft.FSharp.Control.IEvent");

                writer.WriteString("<");
                WriteTypeReference(handler, writer);
                writer.WriteString(",");
                writer.WriteLine();
                writer.WriteString("    ");
                if (args == null)
                {
                    writer.WriteReferenceLink("T:System.EventArgs");
                }
                else
                {
                    WriteTypeReference(args, writer);
                }
                writer.WriteString(">");
                if (i == 0)
                    writer.WriteLine();
            }
        }


        public override void WriteFieldSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {

            string name = (string)reflection.Evaluate(apiNameExpression);
            bool isStatic = (bool)reflection.Evaluate(apiIsStaticExpression);
            bool isLiteral = (bool)reflection.Evaluate(apiIsLiteralFieldExpression);
            bool isInitOnly = (bool)reflection.Evaluate(apiIsInitOnlyFieldExpression);
            bool isSerialized = (bool)reflection.Evaluate(apiIsSerializedFieldExpression);

            if (!isSerialized) WriteAttribute("T:System.NonSerializedAttribute", writer);
            WriteAttributes(reflection, writer);


            if (isStatic)
            {
                writer.WriteKeyword("static");
                writer.WriteString(" ");
            }
            writer.WriteKeyword("val");
            writer.WriteString(" ");

            if (!isInitOnly)
            {
                writer.WriteKeyword("mutable");
                writer.WriteString(" ");
            }

            WriteVisibility(reflection, writer);

            writer.WriteIdentifier(name);

            writer.WriteString(": ");
            WriteReturnValue(reflection, writer);

        }


        private void WriteDotNetObject(XPathNavigator reflection, SyntaxWriter writer,
            string kind)
        {
            string name = reflection.Evaluate(apiNameExpression).ToString();
            bool isSerializable = (bool)reflection.Evaluate(apiIsSerializableTypeExpression);
            XPathNodeIterator implements = reflection.Select(apiImplementedInterfacesExpression);
            XPathNavigator baseClass = reflection.SelectSingleNode(apiBaseClassExpression);
            bool hasBaseClass = (baseClass != null) && !((bool)baseClass.Evaluate(typeIsObjectExpression));

            // CLR considers interfaces abstract.
            bool isAbstract = (bool)reflection.Evaluate(apiIsAbstractTypeExpression) && kind != "interface";
            bool isSealed = (bool)reflection.Evaluate(apiIsSealedTypeExpression);

            if (isAbstract)
                WriteAttribute("T:Microsoft.FSharp.Core.AbstractClassAttribute", writer);
            if (isSealed)
                WriteAttribute("T:Microsoft.FSharp.Core.SealedAttribute", writer);

            if (isSerializable) WriteAttribute("T:System.SerializableAttribute", writer);
            WriteAttributes(reflection, writer);

            writer.WriteKeyword("type");
            writer.WriteString(" ");
            writer.WriteIdentifier(name);
            WriteGenericTemplates(reflection, writer);
            writer.WriteString(" =  ");

            if (hasBaseClass || implements.Count != 0)
            {
                writer.WriteLine();
                writer.WriteString("    ");
            }
            writer.WriteKeyword(kind);

            if (hasBaseClass || implements.Count != 0)
            {
                writer.WriteLine();
            }

            if (hasBaseClass)
            {
                writer.WriteString("        ");
                writer.WriteKeyword("inherit");
                writer.WriteString(" ");
                WriteTypeReference(baseClass, writer);
                writer.WriteLine();
            }


            while (implements.MoveNext())
            {
                XPathNavigator implement = implements.Current;
                writer.WriteString("        ");
                writer.WriteKeyword("interface");
                writer.WriteString(" ");
                WriteTypeReference(implement, writer);
                writer.WriteLine();
            }

            if (hasBaseClass || implements.Count != 0)
            {
                writer.WriteString("    ");
            }
            else
            {
                writer.WriteString(" ");
            }

            writer.WriteKeyword("end");

        }
            
        // Visibility

        private void WriteVisibility(XPathNavigator reflection, SyntaxWriter writer)
        {

            string visibility = reflection.Evaluate(apiVisibilityExpression).ToString();
            WriteVisibility(visibility, writer);
        }


        private Dictionary<string, string> visibilityDictionary = new Dictionary<string, string>()
            {
                { "public", null }, // Default in F#, so unnecessary.
                { "family", null }, // Not supported in F#, section 8.8 in F# spec.
                { "family or assembly", null }, // Not supported in F#, section 8.8 in F# spec. 
                { "family and assembly", null }, // Not supported in F#, section 8.8 in F# spec.
                { "assembly", "internal" },
                { "private", "private" },
            };

        // DONE
        private void WriteVisibility(string visibility, SyntaxWriter writer)
        {

            if(visibilityDictionary.ContainsKey(visibility) && visibilityDictionary[visibility] != null)
            {
                writer.WriteKeyword(visibilityDictionary[visibility]);
                writer.WriteString(" ");
            }

        }

        // Write member | abstract | override
        private void WriteMemberKeyword(XPathNavigator reflection, SyntaxWriter writer)
        {
            bool isOverride = (bool)reflection.Evaluate(apiIsOverrideExpression);
            bool isAbstract = (bool)reflection.Evaluate(apiIsAbstractProcedureExpression);

            if (isOverride)
            {
                writer.WriteKeyword("override");
            }
            else if (isAbstract)
            {
                writer.WriteKeyword("abstract");
            }
            else
            {
                writer.WriteKeyword("member");
            }
            writer.WriteString(" ");

            return;
        }

        // Attributes

        private void WriteAttribute(string reference, SyntaxWriter writer)
        {
            writer.WriteString("[<");
            writer.WriteReferenceLink(reference);
            writer.WriteString(">]");
            writer.WriteLine();
        }


        // Initial version
        private void WriteAttributes(XPathNavigator reflection, SyntaxWriter writer)
        {

            XPathNodeIterator attributes = (XPathNodeIterator)reflection.Evaluate(apiAttributesExpression);

            foreach (XPathNavigator attribute in attributes)
            {

                XPathNavigator type = attribute.SelectSingleNode(attributeTypeExpression);
                if (type.GetAttribute("api", String.Empty) == "T:System.Runtime.CompilerServices.ExtensionAttribute") continue;

                writer.WriteString("[<");
                WriteTypeReference(type, writer);

                XPathNodeIterator arguments = (XPathNodeIterator)attribute.Select(attributeArgumentsExpression);
                XPathNodeIterator assignments = (XPathNodeIterator)attribute.Select(attributeAssignmentsExpression);

                if ((arguments.Count > 0) || (assignments.Count > 0))
                {
                    writer.WriteString("(");
                    while (arguments.MoveNext())
                    {
                        XPathNavigator argument = arguments.Current;
                        if (arguments.CurrentPosition > 1)
                        {
                            writer.WriteString(", ");
                            if (writer.Position > maxPosition)
                            {
                                writer.WriteLine();
                                writer.WriteString("    ");
                            }
                        }
                        WriteValue(argument, writer);
                    }
                    if ((arguments.Count > 0) && (assignments.Count > 0)) writer.WriteString(", ");
                    while (assignments.MoveNext())
                    {
                        XPathNavigator assignment = assignments.Current;
                        if (assignments.CurrentPosition > 1)
                        {
                            writer.WriteString(", ");
                            if (writer.Position > maxPosition)
                            {
                                writer.WriteLine();
                                writer.WriteString("    ");
                            }
                        }
                        writer.WriteString((string)assignment.Evaluate(assignmentNameExpression));
                        writer.WriteString(" = ");
                        WriteValue(assignment, writer);

                    }
                    writer.WriteString(")");
                }

                writer.WriteString(">]");
                writer.WriteLine();
            }

        }

        private void WriteValue(XPathNavigator parent, SyntaxWriter writer)
        {

            XPathNavigator type = parent.SelectSingleNode(attributeTypeExpression);
            XPathNavigator value = parent.SelectSingleNode(valueExpression);
            if (value == null) Console.WriteLine("null value");

            switch (value.LocalName)
            {
                case "nullValue":
                    writer.WriteKeyword("null");
                    break;
                case "typeValue":
                    writer.WriteKeyword("typeof");
                    writer.WriteString("(");
                    WriteTypeReference(value.SelectSingleNode(typeExpression), writer);
                    writer.WriteString(")");
                    break;
                case "enumValue":
                    XPathNodeIterator fields = value.SelectChildren(XPathNodeType.Element);
                    while (fields.MoveNext())
                    {
                        string name = fields.Current.GetAttribute("name", String.Empty);
                        if (fields.CurrentPosition > 1) writer.WriteString("|");
                        WriteTypeReference(type, writer);
                        writer.WriteString(".");
                        writer.WriteString(name);
                    }
                    break;
                case "value":
                    string text = value.Value;
                    string typeId = type.GetAttribute("api", String.Empty);
                    switch (typeId)
                    {
                        case "T:System.String":
                            writer.WriteString("\"");
                            writer.WriteString(text);
                            writer.WriteString("\"");
                            break;
                        case "T:System.Boolean":
                            bool bool_value = Convert.ToBoolean(text);
                            if (bool_value)
                            {
                                writer.WriteKeyword("true");
                            }
                            else
                            {
                                writer.WriteKeyword("false");
                            }
                            break;
                        case "T:System.Char":
                            writer.WriteString("'");
                            writer.WriteString(text);
                            writer.WriteString("'");
                            break;
                    }
                    break;
            }
        }


        // Generics

        private void WriteGenericTemplates(XPathNavigator reflection, SyntaxWriter writer)
        {

            XPathNodeIterator templates = (XPathNodeIterator)reflection.Evaluate(apiTemplatesExpression);

            if (templates.Count == 0) return;
            writer.WriteString("<");
            while (templates.MoveNext())
            {
                XPathNavigator template = templates.Current;
                string name = template.GetAttribute("name", String.Empty);
                writer.WriteString("'");
                writer.WriteString(name);
                if (templates.CurrentPosition < templates.Count) writer.WriteString(", ");
            }
            WriteGenericTemplateConstraints(reflection, writer);
            writer.WriteString(">");
        }

        private void WriteGenericTemplateConstraints(XPathNavigator reflection, SyntaxWriter writer)
        {

            XPathNodeIterator templates = reflection.Select(apiTemplatesExpression);

            if (templates.Count == 0) return;

            foreach (XPathNavigator template in templates)
            {

                bool constrained = (bool)template.Evaluate(templateIsConstrainedExpression);
                if (constrained)
                {
                    string name = (string)template.Evaluate(templateNameExpression);

                    writer.WriteString(" ");
                    writer.WriteKeyword("when");
                    writer.WriteString(" '");
                    writer.WriteString(name);
                    writer.WriteString(" : ");
                }
                else
                {
                    continue;
                }

                bool value = (bool)template.Evaluate(templateIsValueTypeExpression);
                bool reference = (bool)template.Evaluate(templateIsReferenceTypeExpression);
                bool constructor = (bool)template.Evaluate(templateIsConstructableExpression);
                XPathNodeIterator constraints = template.Select(templateConstraintsExpression);

                // keep track of whether there is a previous constraint, so we know whether to put a comma
                bool previous = false;

                if (value)
                {
                    if (previous) writer.WriteString(", ");
                    writer.WriteKeyword("struct");
                    previous = true;
                }

                if (reference)
                {
                    if (previous) writer.WriteString(", ");
                    writer.WriteKeyword("not struct");
                    previous = true;
                }

                if (constructor)
                {
                    if (previous) writer.WriteString(", ");
                    writer.WriteKeyword("new");
                    writer.WriteString("()");
                    previous = true;
                }

                foreach (XPathNavigator constraint in constraints)
                {
                    if (previous) writer.WriteString(" and ");
                    WriteTypeReference(constraint, writer);
                    previous = true;
                }
                
            }

        }

        // Parameters

        private void WriteParameters(XPathNavigator reflection, SyntaxWriter writer)
        {

            XPathNodeIterator parameters = reflection.Select(apiParametersExpression);

            if (parameters.Count > 0)
            {
                WriteParameters(parameters, reflection, writer);
            }
            else
            {
                writer.WriteKeyword("unit");
                writer.WriteString(" ");
            }
            return;
        }



        private void WriteParameters(XPathNodeIterator parameters, XPathNavigator reflection, SyntaxWriter writer)
        {

            bool isExtension = (bool)reflection.Evaluate(apiIsExtensionMethod);
            writer.WriteLine();


            while (parameters.MoveNext())
            {
                XPathNavigator parameter = parameters.Current;

                string name = (string)parameter.Evaluate(parameterNameExpression);
                bool isOut = (bool)parameter.Evaluate(parameterIsOutExpression);
                bool isRef = (bool)parameter.Evaluate(parameterIsRefExpression);
                XPathNavigator type = parameter.SelectSingleNode(parameterTypeExpression);
                writer.WriteString("        ");
                writer.WriteParameter(name);
                writer.WriteString(":");
                WriteTypeReference(type, writer);
                if (isOut || isRef)
                {
                    writer.WriteString(" ");
                    writer.WriteKeyword("byref");
                }
                if (parameters.CurrentPosition != parameters.Count)
                {
                    writer.WriteString(" * ");
                    writer.WriteLine();
                }
                else
                {
                    writer.WriteString(" ");
                }
            }

        }

        // Return Value

        private void WriteReturnValue(XPathNavigator reflection, SyntaxWriter writer)
        {

            XPathNavigator type = reflection.SelectSingleNode(apiReturnTypeExpression);

            if (type == null)
            {
                writer.WriteKeyword("unit");
            }
            else
            {
                WriteTypeReference(type, writer);
            }
        }

        // References

        private void WriteTypeReference(XPathNavigator reference, SyntaxWriter writer)
        {
            switch (reference.LocalName)
            {
                case "arrayOf":
                    int rank = Convert.ToInt32(reference.GetAttribute("rank", String.Empty));
                    XPathNavigator element = reference.SelectSingleNode(typeExpression);
                    WriteTypeReference(element, writer);
                    writer.WriteString("[");
                    for (int i = 1; i < rank; i++) { writer.WriteString(","); }
                    writer.WriteString("]");
                    break;
                case "pointerTo":
                    XPathNavigator pointee = reference.SelectSingleNode(typeExpression);
                    writer.WriteKeyword("nativeptr");
                    writer.WriteString("<");
                    WriteTypeReference(pointee, writer);
                    writer.WriteString(">");
                    break;
                case "referenceTo":
                    XPathNavigator referee = reference.SelectSingleNode(typeExpression);
                    WriteTypeReference(referee, writer);
                    break;
                case "type":
                    string id = reference.GetAttribute("api", String.Empty);
                    WriteNormalTypeReference(id, writer);
                    XPathNodeIterator typeModifiers = reference.Select(typeModifiersExpression);
                    while (typeModifiers.MoveNext())
                    {
                        WriteTypeReference(typeModifiers.Current, writer);
                    }
                    break;
                case "template":
                    string name = reference.GetAttribute("name", String.Empty);
                    writer.WriteString("'");
                    writer.WriteString(name);
                    XPathNodeIterator modifiers = reference.Select(typeModifiersExpression);
                    while (modifiers.MoveNext())
                    {
                        WriteTypeReference(modifiers.Current, writer);
                    }
                    break;
                case "specialization":
                    writer.WriteString("<");
                    XPathNodeIterator arguments = reference.Select(specializationArgumentsExpression);
                    while (arguments.MoveNext())
                    {
                        if (arguments.CurrentPosition > 1) writer.WriteString(", ");
                        WriteTypeReference(arguments.Current, writer);
                    }
                    writer.WriteString(">");
                    break;
            }
        }

        // DONE
        private void WriteNormalTypeReference(string api, SyntaxWriter writer)
        {
            switch (api)
            {
                case "T:System.Void":
                    writer.WriteReferenceLink(api, "unit");
                    break;
                case "T:System.String":
                    writer.WriteReferenceLink(api, "string");
                    break;
                case "T:System.Boolean":
                    writer.WriteReferenceLink(api, "bool");
                    break;
                case "T:System.Byte":
                    writer.WriteReferenceLink(api, "byte");
                    break;
                case "T:System.SByte":
                    writer.WriteReferenceLink(api, "sbyte");
                    break;
                case "T:System.Char":
                    writer.WriteReferenceLink(api, "char");
                    break;
                case "T:System.Int16":
                    writer.WriteReferenceLink(api, "int16");
                    break;
                case "T:System.Int32":
                    writer.WriteReferenceLink(api, "int");
                    break;
                case "T:System.Int64":
                    writer.WriteReferenceLink(api, "int64");
                    break;
                case "T:System.UInt16":
                    writer.WriteReferenceLink(api, "uint16");
                    break;
                case "T:System.UInt32":
                    writer.WriteReferenceLink(api, "uint32");
                    break;
                case "T:System.UInt64":
                    writer.WriteReferenceLink(api, "uint64");
                    break;
                case "T:System.Single":
                    writer.WriteReferenceLink(api, "float32");
                    break;
                case "T:System.Double":
                    writer.WriteReferenceLink(api, "float");
                    break;
                case "T:System.Decimal":
                    writer.WriteReferenceLink(api, "decimal");
                    break;
                default:
                    writer.WriteReferenceLink(api);
                    break;
            }
        }


    }

}
