#pragma once

#define COMBINE_INT32(a1, a2) ((((int64_t) a2) << 32) | (uint32_t) a1)

namespace WrapMode
{
	const int None = 0;
	const int Letters = 1;
	const int Words = 2;
}

namespace Styles
{
	const int Regular = 0;
	const int Bold = 1;
	const int Italic = 2;
	const int Underline = 4;
	const int Strikeout = 8;
}

// IN:
// text - the text to be measured as a C string
// textlen - the length of "text"
// typeface - name of the font/typeface (e.g. "Tahoma") as a C string
// typefacelen - the length of "typeface"
// pointsize - font size in points
// styles - bitwise combination of the Styles constants above
// wrapmode - one of the WrapMode constants above
// wrapwidth - the width to wrap at if wrapmode is not WrapMode::None
// RETURNS:
// the width and height as ints combined into a long
extern "C" int64_t MeasureString(char* text, int32_t textlen, char* typeface, int32_t typefacelen, double pointsize, int32_t styles, int32_t wrapmode, int32_t wrapwidth);

// IN:
// text - the text to be drawn as a C string
// textlen - the length of "text"
// typeface - name of the font/typeface (e.g. "Tahoma") as a C string
// typefacelen - the length of "typeface"
// pointsize - font size in points
// styles - bitwise combination of the Styles constants above
// wrapmode - one of the WrapMode constants above
// wrapwidth - the width to wrap at if wrapmode is not WrapMode::None
// r - red channel of main text colour in range 0 to 1
// g - green channel of main text colour in range 0 to 1
// b - blue channel of main text colour in range 0 to 1
// a - alpha channel of main text colour in range 0 to 1
// w - width of image in pixels (obtained from MeasureString)
// h - height of image in pixels (obtained from MeasureString)
// OUT:
// buffer - a buffer allocated to w * h * 4 that will take RGBA pixels
extern "C" void DrawString(char* text, int32_t textlen, char* typeface, int32_t typefacelen, double pointsize, int32_t styles, int32_t wrapmode, int32_t wrapwidth, double r, double g, double b, double a, int32_t w, int32_t h, unsigned char* buffer);

