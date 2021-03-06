// Copyright � Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace Microsoft.Ddue.Tools {


	public class JSharpDeclarationSyntaxGenerator : DeclarationSyntaxGeneratorTemplate {

        public JSharpDeclarationSyntaxGenerator (XPathNavigator configuration) : base(configuration) {
            if (String.IsNullOrEmpty(Language)) Language = "JSharp";
        }

		// private static string unsupportedGeneric = "UnsupportedGeneric_JSharp";

		public override void WriteNamespaceSyntax (XPathNavigator reflection, SyntaxWriter writer) {

			string name = reflection.Evaluate(apiNameExpression).ToString();

			writer.WriteKeyword("package");
			writer.WriteString(" ");
			writer.WriteIdentifier(name);
		}


		public override void WriteClassSyntax (XPathNavigator reflection, SyntaxWriter writer) {

			if (IsUnsupportedGeneric(reflection, writer)) return;

			string name = reflection.Evaluate(apiNameExpression).ToString();
			bool isAbstract = (bool) reflection.Evaluate(apiIsAbstractTypeExpression);
			bool isSealed = (bool) reflection.Evaluate(apiIsSealedTypeExpression);
			bool isSerializable = (bool) reflection.Evaluate(apiIsSerializableTypeExpression);

			if (isSerializable) WriteAttribute("T:System.SerializableAttribute", writer);
			WriteAttributes(reflection, writer);
			WriteVisibility(reflection, writer);
			writer.WriteString(" ");
			if (isSealed) {
				writer.WriteKeyword("final");
				writer.WriteString(" ");
			} else if (isAbstract) {
				writer.WriteKeyword("abstract");
				writer.WriteString(" ");
			}
			writer.WriteKeyword("class");
			writer.WriteString(" ");
			writer.WriteIdentifier(name);

			XPathNavigator baseClass = reflection.SelectSingleNode(apiBaseClassExpression);
			if ((baseClass != null) && !((bool) baseClass.Evaluate(typeIsObjectExpression))) {
				writer.WriteString(" ");
				writer.WriteKeyword("extends");
				writer.WriteString(" ");
				WriteTypeReference(baseClass, writer);
			}

			WriteImplementedInterfaces(reflection, writer);

		}


		public override void WriteStructureSyntax (XPathNavigator reflection, SyntaxWriter writer) {

			if (IsUnsupportedGeneric(reflection, writer)) return;

			string name = (string) reflection.Evaluate(apiNameExpression);
			bool isSerializable = (bool) reflection.Evaluate(apiIsSerializableTypeExpression);

			if (isSerializable) WriteAttribute("T:System.SerializableAttribute", writer);
			WriteAttributes(reflection, writer);
			WriteVisibility(reflection, writer);
			writer.WriteString(" ");
			writer.WriteKeyword("final");
			writer.WriteString(" ");
			writer.WriteKeyword("class");
			writer.WriteString(" ");
			writer.WriteIdentifier(name);

			writer.WriteString(" ");
			writer.WriteKeyword("extends");
			writer.WriteString(" ");
			writer.WriteReferenceLink("T:System.ValueType");

			WriteImplementedInterfaces(reflection, writer);
		}

		public override void WriteInterfaceSyntax (XPathNavigator reflection, SyntaxWriter writer) {

			if (IsUnsupportedGeneric(reflection, writer)) return;

			string name = (string) reflection.Evaluate(apiNameExpression);

			WriteAttributes(reflection, writer);
			WriteVisibility(reflection, writer);
			writer.WriteString(" ");
			writer.WriteKeyword("interface");
			writer.WriteString(" ");
			writer.WriteIdentifier(name);
			WriteImplementedInterfaces("extends", reflection, writer);
		}

		public override void WriteDelegateSyntax (XPathNavigator reflection, SyntaxWriter writer) {

			if (IsUnsupportedUnsafe(reflection, writer)) return;
			if (IsUnsupportedGeneric(reflection, writer)) return;

			string name = (string) reflection.Evaluate(apiNameExpression);
			bool isSerializable = (bool) reflection.Evaluate(apiIsSerializableTypeExpression);

			writer.WriteString("/** @delegate */");
			writer.WriteLine();

			if (isSerializable) WriteAttribute("T:System.SerializableAttribute", writer);
			WriteAttributes(reflection, writer);
			WriteVisibility(reflection, writer);
			writer.WriteString(" ");
			writer.WriteKeyword("delegate");
			writer.WriteString(" ");
			WriteReturnValue(reflection, writer);
			writer.WriteString(" ");
			writer.WriteIdentifier(name);
			WriteMethodParameters(reflection, writer);
			
		}

		public override void WriteEnumerationSyntax (XPathNavigator reflection, SyntaxWriter writer) {

			string name = (string) reflection.Evaluate(apiNameExpression);
			bool isSerializable = (bool) reflection.Evaluate(apiIsSerializableTypeExpression);

			if (isSerializable) WriteAttribute("T:System.SerializableAttribute", writer);
			WriteAttributes(reflection, writer);
			WriteVisibility(reflection, writer);
			writer.WriteString(" ");
			writer.WriteKeyword("enum");
			writer.WriteString(" ");
			writer.WriteIdentifier(name);
		}

		public override void WriteConstructorSyntax (XPathNavigator reflection, SyntaxWriter writer) {

			if (IsUnsupportedUnsafe(reflection, writer)) return;

			string name = (string) reflection.Evaluate(apiContainingTypeNameExpression);
			bool isStatic = (bool) reflection.Evaluate(apiIsStaticExpression);

			if (isStatic) {
				// no static constructors in Java
				writer.WriteMessage("UnsupportedStaticConstructor_" + Language);
				return;
			}

			WriteAttributes(reflection, writer);
			WriteVisibility(reflection, writer);
			writer.WriteString(" ");
			writer.WriteIdentifier(name);
			WriteMethodParameters(reflection, writer);

		}

		public override void WriteMethodSyntax (XPathNavigator reflection, SyntaxWriter writer) {
			bool isSpecialName = (bool) reflection.Evaluate(apiIsSpecialExpression);

			if (isSpecialName) {
				writer.WriteMessage("UnsupportedOperator_" + Language);
			} else {
				WriteNormalMethodSyntax(reflection, writer);
			}
		}

		public override void WriteNormalMethodSyntax (XPathNavigator reflection, SyntaxWriter writer) {

			if (IsUnsupportedUnsafe(reflection, writer)) return;
			if (IsUnsupportedGeneric(reflection, writer)) return;
			if (IsUnsupportedExplicit(reflection, writer)) return;

			string name = (string) reflection.Evaluate(apiNameExpression);

			WriteAttributes(reflection, writer);
			WriteProcedureModifiers(reflection, writer);
			WriteReturnValue(reflection, writer);
			writer.WriteString(" ");
			writer.WriteIdentifier(name);
			WriteMethodParameters(reflection, writer);
		}

        private void WriteNamedNormalMethodSyntax (string name, XPathNavigator reflection, SyntaxWriter writer) {


        }

        public override void WritePropertySyntax (XPathNavigator reflection, SyntaxWriter writer) {

			if (IsUnsupportedUnsafe(reflection, writer)) return;
			if (IsUnsupportedExplicit(reflection, writer)) return;

			string name = (string) reflection.Evaluate(apiNameExpression);
			bool isGettable = (bool) reflection.Evaluate(apiIsReadPropertyExpression);
			bool isSettable = (bool) reflection.Evaluate(apiIsWritePropertyExpression);

			if (isGettable) {
				writer.WriteString("/** @property */");
				writer.WriteLine();
				// write getter method
                WriteAttributes(reflection, writer);
                WriteProcedureModifiers(reflection, writer);
                WriteReturnValue(reflection, writer);
                writer.WriteString(" ");
                writer.WriteIdentifier("get_" + name);
                WriteMethodParameters(reflection, writer);
                writer.WriteLine();
			}

			if (isSettable) {
				writer.WriteString("/** @property */");
				writer.WriteLine();
				// write setter method
                WriteAttributes(reflection, writer);
                WriteProcedureModifiers(reflection, writer);
                writer.WriteString(" ");
                writer.WriteKeyword("void");
                writer.WriteString(" ");
                writer.WriteIdentifier("set_" + name);
                // parameters
                writer.WriteString("(");
                WriteReturnValue(reflection, writer);
                writer.WriteString(" ");
                writer.WriteParameter("value");
                writer.WriteString(")");
                // end parameters
                writer.WriteLine();
			}

		}

		public override void WriteEventSyntax (XPathNavigator reflection, SyntaxWriter writer) {

			if (IsUnsupportedUnsafe(reflection, writer)) return;

			string name = (string) reflection.Evaluate(apiNameExpression);
			XPathNavigator handler = reflection.SelectSingleNode(apiHandlerOfEventExpression);

			writer.WriteString("/** @event */");
			writer.WriteLine();
			// add_ method declaration
            WriteAttributes(reflection, writer);
            WriteProcedureModifiers(reflection, writer);
            WriteReturnValue(reflection, writer);
            writer.WriteString(" ");
            writer.WriteIdentifier("add_" + name);
            writer.WriteString(" (");
            WriteTypeReference(handler, writer);
            writer.WriteString(" ");
            writer.WriteParameter("value");
            writer.WriteString(")");
			writer.WriteLine();

			writer.WriteString("/** @event */");
			writer.WriteLine();
			// remove_ method declaration
            WriteAttributes(reflection, writer);
            WriteProcedureModifiers(reflection, writer);
            WriteReturnValue(reflection, writer);
            writer.WriteString(" ");
            writer.WriteIdentifier("remove_" + name);
            writer.WriteString(" (");
            WriteTypeReference(handler, writer);
            writer.WriteString(" ");
            writer.WriteParameter("value");
            writer.WriteString(")");
			writer.WriteLine();

		}

		private void WriteProcedureModifiers (XPathNavigator reflection, SyntaxWriter writer) {

			// interface members don't get modified
			string typeSubgroup = (string) reflection.Evaluate(apiContainingTypeSubgroupExpression);
			if (typeSubgroup == "interface") return;

			bool isStatic = (bool) reflection.Evaluate(apiIsStaticExpression);
			bool isVirtual = (bool) reflection.Evaluate(apiIsVirtualExpression);
			bool isAbstract = (bool) reflection.Evaluate(apiIsAbstractProcedureExpression);
			bool isFinal = (bool) reflection.Evaluate(apiIsFinalExpression);
			// bool isOverride = (bool) reflection.Evaluate(apiIsOverrideExpression);

			WriteVisibility(reflection, writer);
			writer.WriteString(" ");
			if (isStatic) {
				writer.WriteKeyword("static");
				writer.WriteString(" ");
			} else {
				if (isVirtual) {
					if (isAbstract) {
						writer.WriteKeyword("abstract");
						writer.WriteString(" ");
					} else if (isFinal) {
						writer.WriteKeyword("final");
						writer.WriteString(" ");
					}
				}
			}

		}

		public override void WriteFieldSyntax (XPathNavigator reflection, SyntaxWriter writer) {

			string name = (string) reflection.Evaluate(apiNameExpression);
			bool isStatic = (bool) reflection.Evaluate(apiIsStaticExpression);
			bool isLiteral = (bool) reflection.Evaluate(apiIsLiteralFieldExpression);
			bool isInitOnly = (bool) reflection.Evaluate(apiIsInitOnlyFieldExpression);
			bool isSerialized = (bool) reflection.Evaluate(apiIsSerializedFieldExpression);

			if (!isSerialized) WriteAttribute("T:System.NonSerializedAttribute", writer);
			WriteAttributes(reflection, writer);
			WriteVisibility(reflection, writer);
			writer.WriteString(" ");
			// Java doesn't support literals as distinct from static initonly
			if (isStatic) {
				writer.WriteKeyword("static");
				writer.WriteString(" ");
			}
			if (isLiteral || isInitOnly) {
				writer.WriteKeyword("final");
				writer.WriteString(" ");
			}
			WriteReturnValue(reflection, writer);
			writer.WriteString(" ");
			writer.WriteIdentifier(name);

		}

		// Visibility

		protected override void WriteVisibility (XPathNavigator reflection, SyntaxWriter writer) {

			string visibility = reflection.Evaluate(apiVisibilityExpression).ToString();

			switch (visibility) {
				case "public":
					writer.WriteKeyword("public");
				break;
				case "family":
					// in Java, protected = family or assembly
					writer.WriteKeyword("protected");
				break;
				case "family or assembly":
					writer.WriteKeyword("protected");
				break;
				case "assembly":
					// no assembly-only access in Java
				break;
				case "private":
					writer.WriteKeyword("private");
				break;
			}

		}

		// Attributes

		private void WriteAttribute (string reference, SyntaxWriter writer) {
			WriteAttribute(reference, writer, true);
		}

		private void WriteAttribute (string reference, SyntaxWriter writer, bool newline) {
			writer.WriteString("/** @attribute ");
			writer.WriteReferenceLink(reference);
			writer.WriteString(" */ ");
			if (newline) writer.WriteLine();
		}


		private void WriteAttributes (XPathNavigator reflection, SyntaxWriter writer) {

			XPathNodeIterator attributes = (XPathNodeIterator) reflection.Evaluate(apiAttributesExpression);

			foreach (XPathNavigator attribute in attributes) {

				XPathNavigator type = attribute.SelectSingleNode(attributeTypeExpression);

				writer.WriteString("/** @attribute ");
				WriteTypeReference(type, writer);

				XPathNodeIterator arguments = (XPathNodeIterator) attribute.Select(attributeArgumentsExpression);
				XPathNodeIterator assignments = (XPathNodeIterator) attribute.Select(attributeAssignmentsExpression);

				if ((arguments.Count > 0) || (assignments.Count > 0)) {
					writer.WriteString("(");
					while (arguments.MoveNext()) {
						XPathNavigator argument = arguments.Current;
						if (arguments.CurrentPosition > 1) writer.WriteString(", ");
						WriteValue(argument, writer);
					}
					if ((arguments.Count > 0) && (assignments.Count > 0)) writer.WriteString(", ");
					while (assignments.MoveNext()) {
						XPathNavigator assignment = assignments.Current;
						if (assignments.CurrentPosition > 1) writer.WriteString(", ");
						writer.WriteString((string) assignment.Evaluate(assignmentNameExpression));
						writer.WriteString(" = ");
						WriteValue(assignment, writer);
					}
					writer.WriteString(")");
				}

				writer.WriteString(" */");
				writer.WriteLine();
			}

		}

		private void WriteValue (XPathNavigator parent, SyntaxWriter writer) {

			XPathNavigator type = parent.SelectSingleNode(attributeTypeExpression);
			XPathNavigator value = parent.SelectSingleNode(valueExpression);
			// if (value == null) Console.WriteLine("null value");

			switch (value.LocalName) {
				case "nullValue":
					writer.WriteKeyword("null");
				break;
				case "typeValue":
					// this isn't really supported in J#; there is no compile-time way to get a type representation
					// writer.WriteKeyword("typeof");
					// writer.WriteString("(");
					WriteTypeReference(value.SelectSingleNode(typeExpression), writer);
					// writer.WriteString(")");
				break;
				case "enumValue":
					XPathNodeIterator fields = value.SelectChildren(XPathNodeType.Element);
					while (fields.MoveNext()) {
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
					switch (typeId) {
						case "T:System.String":
							writer.WriteString("\"");
							writer.WriteString(text);
							writer.WriteString("\"");
						break;
						case "T:System.Boolean":
							bool bool_value = Convert.ToBoolean(text);
							if (bool_value) {
								writer.WriteKeyword("true");
							} else {
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

		// Interfaces

		private void WriteImplementedInterfaces (XPathNavigator reflection, SyntaxWriter writer) {
			WriteImplementedInterfaces("implements", reflection, writer);
		}

		private void WriteImplementedInterfaces (string keyword, XPathNavigator reflection, SyntaxWriter writer) {

			XPathNodeIterator implements = reflection.Select(apiImplementedInterfacesExpression);

			if (implements.Count == 0) return;

			writer.WriteString(" ");
			writer.WriteKeyword(keyword);
			writer.WriteString(" ");

			while (implements.MoveNext()) {
				XPathNavigator implement = implements.Current;
				WriteTypeReference(implement, writer);
                if (implements.CurrentPosition < implements.Count) {
                    writer.WriteString(", ");
                    if (writer.Position > maxPosition) {
                        writer.WriteLine();
                        writer.WriteString("\t");
                    }
                }
			}

		}
		// Parameters

		private void WriteMethodParameters (XPathNavigator reflection, SyntaxWriter writer) {

			XPathNodeIterator parameters = reflection.Select(apiParametersExpression);

			writer.WriteString("(");
			if (parameters.Count > 0) {
				writer.WriteLine();
				WriteParameters(parameters, reflection, writer);
			}
			writer.WriteString(")");

		}

             
		private void WriteParameters (XPathNodeIterator parameters, XPathNavigator reflection, SyntaxWriter writer) {

			while (parameters.MoveNext()) {
				XPathNavigator parameter = parameters.Current;

				string name = (string) parameter.Evaluate(parameterNameExpression);
				XPathNavigator type = parameter.SelectSingleNode(parameterTypeExpression);
				bool isIn = (bool) parameter.Evaluate(parameterIsInExpression);
				bool isOut = (bool) parameter.Evaluate(parameterIsOutExpression);
				bool isRef = (bool) parameter.Evaluate(parameterIsRefExpression);
				// bool isParamArray = (bool) parameter.Evaluate(parameterIsParamArrayExpression);

				writer.WriteString("\t");

				if (isIn) WriteAttribute("T:System.Runtime.InteropServices.InAttribute", writer, false);
				if (isOut) WriteAttribute("T:System.Runtime.InteropServices.OutAttribute", writer, false);
				if (isRef) {
					writer.WriteString("/** @ref */");
				}

				WriteTypeReference(type, writer);
				writer.WriteString(" ");
				writer.WriteParameter(name);

				if (parameters.CurrentPosition < parameters.Count) writer.WriteString(",");
				writer.WriteLine();
			}

		}

		// Return Value

		private void WriteReturnValue (XPathNavigator reflection, SyntaxWriter writer) {

			XPathNavigator type = reflection.SelectSingleNode(apiReturnTypeExpression);

			if (type == null) {
				writer.WriteKeyword("void");
			} else {
				WriteTypeReference(type, writer);
			}
		}

		// References

		private void WriteTypeReference (XPathNavigator reference, SyntaxWriter writer) {
			switch (reference.LocalName) {
				case "arrayOf":
					int rank = Convert.ToInt32( reference.GetAttribute("rank",String.Empty) );
					XPathNavigator element = reference.SelectSingleNode(typeExpression);
					WriteTypeReference(element, writer);
					writer.WriteString("[");
					for (int i=1; i<rank; i++) { writer.WriteString(","); }
					writer.WriteString("]");
				break;
				case "pointerTo":
					XPathNavigator pointee = reference.SelectSingleNode(typeExpression);
					WriteTypeReference(pointee, writer);
					writer.WriteString("*");
				break;
				case "referenceTo":
					XPathNavigator referee = reference.SelectSingleNode(typeExpression);
					WriteTypeReference(referee, writer);
				break;
				case "type":
					string id = reference.GetAttribute("api", String.Empty);
					WriteNormalTypeReference(id, writer);
					XPathNodeIterator typeModifiers = reference.Select(typeModifiersExpression);
					while (typeModifiers.MoveNext()) {
						WriteTypeReference(typeModifiers.Current, writer);
					}
				break;
				case "template":
					string name = reference.GetAttribute("name", String.Empty);
					writer.WriteString(name);
					XPathNodeIterator modifiers = reference.Select(typeModifiersExpression);
					while (modifiers.MoveNext()) {
						WriteTypeReference(modifiers.Current, writer);
					}
				break;
				case "specialization":
					writer.WriteString("<");
					XPathNodeIterator arguments = reference.Select(specializationArgumentsExpression);
					while (arguments.MoveNext()) {
						if (arguments.CurrentPosition > 1) writer.WriteString(", ");
						WriteTypeReference(arguments.Current, writer);
					}
					writer.WriteString(">");
				break;
			}
		}

		private void WriteNormalTypeReference (string reference, SyntaxWriter writer) {
			switch (reference) {
				case "T:System.Void":
					writer.WriteReferenceLink(reference, "void");
				break;
				case "T:System.Boolean":
					writer.WriteReferenceLink(reference, "boolean");
				break;
				case "T:System.Byte":
					writer.WriteReferenceLink(reference, "byte");
				break;
				case "T:System.Char":
					writer.WriteReferenceLink(reference, "char");
				break;
				case "T:System.Int16":
					writer.WriteReferenceLink(reference, "short");
				break;
				case "T:System.Int32":
					writer.WriteReferenceLink(reference, "int");
				break;
				case "T:System.Int64":
					writer.WriteReferenceLink(reference, "long");
				break;
				case "T:System.Single":
					writer.WriteReferenceLink(reference, "float");
				break;
				case "T:System.Double":
					writer.WriteReferenceLink(reference, "double");
				break;
				default:
					writer.WriteReferenceLink(reference);
				break;
			}
		}


	}

}
