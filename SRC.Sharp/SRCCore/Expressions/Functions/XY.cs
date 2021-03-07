using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Expressions
{
	public static class XY
	{
		public static ValueType X(this Expression Expression, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
		{
			switch (pcount)
			{
				case 1:
					{
						pname = GetValueAsString(@params[1], is_term[1]);
						switch (pname ?? "")
						{
							case "目標地点":
								{
									num_result = (double)Commands.SelectedX;
									break;
								}

							case "マウス":
								{
									num_result = (double)GUI.MouseX;
									break;
								}

							default:
								{
									bool localIsDefined33() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

									object argIndex74 = (object)pname;
									if (SRC.UList.IsDefined2(argIndex74))
									{
										Unit localItem220() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

										num_result = (double)localItem220().x;
									}
									else if (localIsDefined33())
									{
										object argIndex73 = (object)pname;
										{
											var withBlock45 = SRC.PList.Item(argIndex73);
											if (withBlock45.Unit is object)
											{
												num_result = (double)withBlock45.Unit.x;
											}
										}
									}

									break;
								}
						}

						break;
					}

				case 0:
					{
						if (Event.SelectedUnitForEvent is object)
						{
							num_result = (double)Event.SelectedUnitForEvent.x;
						}

						break;
					}
			}

			if (etype == ValueType.StringType)
			{
				str_result = GeneralLib.FormatNum(num_result);
				CallFunctionRet = ValueType.StringType;
			}
			else
			{
				CallFunctionRet = ValueType.NumericType;
			}

			return CallFunctionRet;
		}

		public static ValueType Y(this Expression Expression, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
		{
			switch (pcount)
			{
				case 1:
					{
						pname = GetValueAsString(@params[1], is_term[1]);
						switch (pname ?? "")
						{
							case "目標地点":
								{
									num_result = (double)Commands.SelectedY;
									break;
								}

							case "マウス":
								{
									num_result = (double)GUI.MouseY;
									break;
								}

							default:
								{
									bool localIsDefined34() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

									object argIndex76 = (object)pname;
									if (SRC.UList.IsDefined2(argIndex76))
									{
										Unit localItem221() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

										num_result = (double)localItem221().y;
									}
									else if (localIsDefined34())
									{
										object argIndex75 = (object)pname;
										{
											var withBlock46 = SRC.PList.Item(argIndex75);
											if (withBlock46.Unit is object)
											{
												num_result = (double)withBlock46.Unit.y;
											}
										}
									}

									break;
								}
						}

						break;
					}

				case 0:
					{
						if (Event.SelectedUnitForEvent is object)
						{
							num_result = (double)Event.SelectedUnitForEvent.y;
						}

						break;
					}
			}

			if (etype == ValueType.StringType)
			{
				str_result = GeneralLib.FormatNum(num_result);
				CallFunctionRet = ValueType.StringType;
			}
			else
			{
				CallFunctionRet = ValueType.NumericType;
			}

			return CallFunctionRet;
		}
	}
}
