using System;

namespace CS2Interface {
	public abstract partial class IAttribute {
		internal abstract string Name { get; }
		internal abstract Type Type { get; }

		internal abstract uint ToUInt32();

		internal abstract float ToSingle();

		public override abstract string ToString();
	}
	
	public sealed class Attribute<TObject> : IAttribute where TObject : notnull {
		internal override string Name { get; }
		internal override Type Type { get => typeof(TObject); }
		internal TObject Value;

		public Attribute(string name, TObject value) {
			Name = name;
			Value = value;
		}

		internal override uint ToUInt32() => (uint) Convert.ChangeType(Value, typeof(uint));
		internal override float ToSingle() => (float) Convert.ChangeType(Value, typeof(float));
		public override string ToString() => (string) Convert.ChangeType(Value, typeof(string));
	}
}