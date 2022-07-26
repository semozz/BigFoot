using System;
using System.Collections.Generic;
using LitJson;
using Hive5;
using Hive5.Util;

namespace Hive5.Model
{

	/// <summary>
	/// Login data.
	/// </summary>
	public class GetSocialScoresResponseBody : IResponseBody
	{
		public string LastPrizedAt { set; get; }
		public List<SocialScore> Scores { set; get; }

		/// <summary>
		/// Load the specified json.
		/// </summary>
		/// <param name="json">Json.</param>
		public static IResponseBody Load(JsonData json)
		{
			if (json == null)
				return null;

			List<SocialScore> scores;
			
			try
			{
				scores = SocialScore.LoadList( json["scores"] );
			}
			catch (KeyNotFoundException)
			{
				scores = null;
			}

			return new GetSocialScoresResponseBody() {
				Scores 			= scores,
			};
		}

	}
}

