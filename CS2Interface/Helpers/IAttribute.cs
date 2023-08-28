using System;

namespace CS2Interface {
    public abstract partial class IAttribute {
		public abstract string Name { get; }
		public abstract Type Type { get; }

		public abstract uint ToUInt32();

        public abstract float ToSingle();

        public override abstract string ToString();
	}
	
	public sealed class Attribute<TObject> : IAttribute where TObject : notnull {
		public override string Name { get; }
		public override Type Type { get => typeof(TObject); }
		public TObject Value;

		public Attribute(string name, TObject value) {
			Name = name;
			Value = value;
		}

        public override uint ToUInt32() => (uint) Convert.ChangeType(Value, typeof(uint));
        public override float ToSingle() => (float) Convert.ChangeType(Value, typeof(float));

        public override string ToString() => (string) Convert.ChangeType(Value, typeof(string));
	}
}