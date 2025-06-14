namespace CS2Interface {
	public abstract class GameObject {
		protected static bool ShouldSerializeAdditionalProperties { get; private set; } = true;
		protected static bool ShouldSerializeDefs { get; private set; } = true ; 

		internal static void SetSerializationProperties(bool should_serialize_additional_properties, bool should_serialize_defs) {
			ShouldSerializeAdditionalProperties = should_serialize_additional_properties;
			ShouldSerializeDefs = should_serialize_defs;
		}

		protected abstract bool SetDefs();
		protected abstract bool SetAdditionalProperties();
	}
}
