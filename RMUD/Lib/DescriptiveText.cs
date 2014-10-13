using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public enum DescriptiveTextType
	{
		TaggedText,
		LambdaText
	}

	public struct DescriptiveText
	{
		private readonly String RawText;
		private readonly Func<Actor,MudObject, String> LambdaText;
		private readonly DescriptiveTextType TextType;

        public override string ToString()
        {
            if (TextType == DescriptiveTextType.TaggedText) return "\"" + RawText + "\"";
            else return "Lambda Text";
        }
		
		public DescriptiveText(String RawText)
		{
			this.RawText = RawText;
			this.LambdaText = null;
			this.TextType = DescriptiveTextType.TaggedText;
		}

		public DescriptiveText(Func<Actor, MudObject, String> LambdaText)
		{
			this.RawText = null;
			this.LambdaText = LambdaText;
			this.TextType = DescriptiveTextType.LambdaText;
		}

		public static implicit operator DescriptiveText(String RawText)
		{
			return new DescriptiveText(RawText);
		}

		public static implicit operator DescriptiveText(Func<Actor, MudObject, String> LambdaText)
		{
			return new DescriptiveText(LambdaText);
		}

		public String Expand(Actor Viewer, MudObject Source)
		{
			switch (TextType)
			{
				case DescriptiveTextType.LambdaText:
					return LambdaText(Viewer, Source);
				case DescriptiveTextType.TaggedText:
					return RawText;
			}
			return null;
		}
	}
}
