#pragma once

#define COMBINE_INT32(a1, a2) ((((int64_t) a2) << 32) | (uint32_t) a1)

#ifdef WIN32
#define EXPORT __declspec(dllexport)
#define CALLMODE __cdecl
#else
#define EXPORT
#define CALLMODE
#endif

namespace WrapMode
{
	const int32_t None = 0;
	const int32_t Letters = 1;
	const int32_t Words = 2;
}

namespace Styles
{
	const int32_t Regular = 0;
	const int32_t Bold = 1;
	const int32_t Italic = 2;
	const int32_t Underline = 4;
	const int32_t Strikeout = 8;
}

namespace Alignment
{
	const int32_t TopLeft = 0;
	const int32_t Top = 1;
	const int32_t TopRight = 2;
	const int32_t Left = 3;
	const int32_t Middle = 4;
	const int32_t Right = 5;
	const int32_t BottomLeft = 6;
	const int32_t Bottom = 7;
	const int32_t BottomRight = 8;
}

// IN:
// text - the text to be measured as a C string
// textlen - the length of "text"
// typeface - name of the font/typeface (e.g. "Tahoma") as a C string
// typefacelen - the length of "typeface"
// pointsize - font size in points
// styles - bitwise combination of the Styles constants above
// alignment - one of the Alignment constants above
// wrapmode - one of the WrapMode constants above
// wrapwidth - the width to wrap at if wrapmode is not WrapMode::None
// RETURNS:
// the width and height as ints combined into a long
extern "C" EXPORT int64_t CALLMODE MeasureString(char* text, int32_t textlen, char* typeface, int32_t typefacelen, double pointsize, int32_t styles, int32_t alignment, int32_t wrapmode, int32_t wrapwidth);

// IN:
// text - the text to be drawn as a C string
// textlen - the length of "text"
// typeface - name of the font/typeface (e.g. "Tahoma") as a C string
// typefacelen - the length of "typeface"
// pointsize - font size in points
// styles - bitwise combination of the Styles constants above
// alignment - one of the Alignment constants above
// wrapmode - one of the WrapMode constants above
// wrapwidth - the width to wrap at if wrapmode is not WrapMode::None
// w - width of image in pixels (obtained from MeasureString)
// h - height of image in pixels (obtained from MeasureString)
// OUT:
// buffer - a buffer allocated to w * h * 4 that will take RGBA pixels
extern "C" EXPORT void CALLMODE DrawString(char* text, int32_t textlen, char* typeface, int32_t typefacelen, double pointsize, int32_t styles, int32_t alignment, int32_t wrapmode, int32_t wrapwidth, double r, double g, double b, double a, int32_t w, int32_t h, unsigned char* buffer);

