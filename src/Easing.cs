using System;

namespace Aiv.Tween {

	/// <summary>
	/// Easing functions.
	/// </summary>
	public static class Easing {

		public static float Linear(float k) {
			return k;
		}

		public static class Quadratic {
			public static float In(float k) {
				return k * k;
			}

			public static float Out(float k) {
				return k * (2 - k);
			}

			public static float InOut(float k) {
				if ((k *= 2) < 1) {
					return 0.5f * k * k;
				}

				return -0.5f * (--k * (k - 2) - 1);
			}
		}

		public static class Cubic {
			public static float In(float k) {
				return k * k * k;
			}

			public static float Out(float k) {
				return --k * k * k + 1;
			}

			public static float InOut(float k) {
				if ((k *= 2) < 1) {
					return 0.5f * k * k * k;
				}

				return 0.5f * ((k -= 2) * k * k + 2);
			}
		}


		public static class Quartic {
			public static float In(float k) {
				return k * k * k * k;
			}

			public static float Out(float k) {
				return 1 - (--k * k * k * k);
			}

			public static float InOut(float k) {
				if ((k *= 2) < 1) {
					return 0.5f * k * k * k * k;
				}

				return -0.5f * ((k -= 2) * k * k * k - 2);
			}
		}

		public static class Quintic {
			public static float In(float k) {
				return k * k * k * k * k;
			}

			public static float Out(float k) {
				return --k * k * k * k * k + 1;
			}

			public static float InOut(float k) {
				if ((k *= 2) < 1) {
					return 0.5f * k * k * k * k * k;
				}

				return 0.5f * ((k -= 2) * k * k * k * k + 2);
			}
		}

		public static class Sinusoidal {
			public static float In(float k) {
				return 1 - (float)Math.Cos(k * Math.PI / 2);
			}

			public static float Out(float k) {
				return (float)Math.Sin(k * Math.PI / 2);
			}

			public static float InOut(float k) {
				return 0.5f * (1 - (float)Math.Cos(Math.PI * k));
			}
		}

		public static class Exponential {
			public static float In(float k) {
				return k == 0 ? 0 : (float)Math.Pow(1024, k - 1);
			}

			public static float Out(float k) {
				return k == 1 ? 1 : 1 - (float)Math.Pow(2, -10 * k);
			}

			public static float InOut(float k) {
				if (k == 0)
					return 0;

				if (k == 1)
					return 1;

				if ((k *= 2) < 1) {
					return 0.5f * (float)Math.Pow(1024, k - 1);
				}

				return 0.5f * (-(float)Math.Pow(2, -10 * (k - 1)) + 2);
			}
		}

		public static class Circular {
			public static float In(float k) {
				return 1 - (float)Math.Sqrt(1 - k * k);
			}

			public static float Out(float k) {
				return (float)Math.Sqrt(1 - (--k * k));
			}

			public static float InOut(float k) {
				if ((k *= 2) < 1) {
					return -0.5f * ((float)Math.Sqrt(1 - k * k) - 1);
				}

				return 0.5f * ((float)Math.Sqrt(1 - (k -= 2) * k) + 1);
			}
		}

		public static class Elastic {
			public static float In(float k) {

				float a = 1f;
				float p = 0.4f;
				float s = p / 4;

				if (k == 0)
					return 0;

				if (k == 1)
					return 1;

				return -(a * (float)Math.Pow(2, 10 * (k -= 1)) * (float)Math.Sin((k - s) * (2 * Math.PI) / p));
			}

			public static float Out(float k) {
				float a = 1f;
				float p = 0.4f;
				float s = p / 4;

				if (k == 0)
					return 0;

				if (k == 1)
					return 1;

				return (a * (float)Math.Pow(2, -10 * k) * (float)Math.Sin((k - s) * (2 * Math.PI) / p) + 1);
			}

			public static float InOut(float k) {
				float a = 1f;
				float p = 0.4f;
				float s = p / 4;

				if (k == 0)
					return 0;

				if (k == 1)
					return 1;

				if ((k *= 2) < 1) {
					return -0.5f * (a * (float)Math.Pow(2, 10 * (k -= 1)) * (float)Math.Sin((k - s) * (2 * (float)Math.PI) / p));
				}

				return a * (float)Math.Pow(2, -10 * (k -= 1)) * (float)Math.Sin((k - s) * (2 * (float)Math.PI) / p) * 0.5f + 1;
			}
		}

		public static class Back {
			public static float In(float k) {
				float s = 1.70158f;
				return k * k * ((s + 1) * k - s);
			}

			public static float Out(float k) {
				float s = 1.70158f;
				return --k * k * ((s + 1) * k + s) + 1;
			}

			public static float InOut(float k) {
				float s = 1.70158f * 1.525f;
				if ((k *= 2) < 1) {
					return 0.5f * (k * k * ((s + 1) * k - s));
				}

				return 0.5f * ((k -= 2) * k * ((s + 1) * k + s) + 2);
			}
		}

		public static class Bounce {
			public static float Out(float k) {
				if (k < (1 / 2.75f)) {
					return 7.5625f * k * k;
				} else if (k < (2 / 2.75f)) {
					return 7.5625f * (k -= (1.5f / 2.75f)) * k + 0.75f;
				} else if (k < (2.5f / 2.75f)) {
					return 7.5625f * (k -= (2.25f / 2.75f)) * k + 0.9375f;
				} else {
					return 7.5625f * (k -= (2.625f / 2.75f)) * k + 0.984375f;
				}
			}

			public static float In(float k) {
				return 1 - Out(1 - k);
			}

			public static float InOut(float k) {
				if (k < 0.5f) {
					return In(k * 2) * 0.5f;
				}

				return Out(k * 2 - 1) * 0.5f + 0.5f;
			}
		}
	}

}
