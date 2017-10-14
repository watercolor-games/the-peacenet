#include <cmath>
#include <cstring>
#include <string>
#include <cairomm/cairomm.h>
#include <pangomm.h>
#include <pangomm/init.h>
#include <pango/pango.h>
#include <pango/pangocairo.h>
#include <windows.h>
#include "ATextRenderer.h"

template <typename T> Glib::RefPtr<Pango::Layout> CreateLayout(std::string text, std::string typeface, double pointsize, int32_t styles, int32_t alignment, int32_t wrapmode, int32_t wrapwidth, T context)
{
	Pango::FontDescription font;
	font.set_family(typeface);
	font.set_weight((styles & Styles::Bold) ? Pango::WEIGHT_BOLD : Pango::WEIGHT_NORMAL);
	font.set_style((styles & Styles::Italic) ? Pango::STYLE_ITALIC : Pango::STYLE_NORMAL);
	font.set_size((int)std::round(pointsize * Pango::SCALE));
	auto layout = Pango::Layout::create(context);
	switch (alignment)
	{
		case Alignment::TopLeft:
		case Alignment::Left:
		case Alignment::BottomLeft:
			layout->set_alignment(Pango::ALIGN_LEFT);
			break;
		case Alignment::Top:
		case Alignment::Middle:
		case Alignment::Bottom:
			layout->set_alignment(Pango::ALIGN_CENTER);
			break;
		case Alignment::TopRight:
		case Alignment::Right:
		case Alignment::BottomRight:
			layout->set_alignment(Pango::ALIGN_RIGHT);
			break;
	}
	switch (wrapmode)
	{
		case WrapMode::None:
			layout->set_width(-1);
			break;
		case WrapMode::Letters:
			layout->set_width(wrapwidth * Pango::SCALE);
			layout->set_wrap(Pango::WrapMode::WRAP_CHAR);
			break;
		case WrapMode::Words:
			layout->set_width(wrapwidth * Pango::SCALE);
			layout->set_wrap(Pango::WrapMode::WRAP_WORD_CHAR);
			break;
	}
	layout->set_text(text);
	layout->set_font_description(font);
	return layout;
}

int64_t MeasureString(char* text, int32_t textlen, char* typeface, int32_t typefacelen, double pointsize, int32_t styles, int32_t alignment, int32_t wrapmode, int32_t wrapwidth)
{
	Pango::init();
	
	// we have to get our hands dirty here - pangomm doesn't provide
	// a way to get a context like this
	auto context = Glib::wrap(pango_font_map_create_context(pango_cairo_font_map_get_default()), false);
	
	auto layout = CreateLayout(std::string(text, textlen), std::string(typeface, typefacelen), pointsize, styles, alignment, wrapmode, wrapwidth, context);
	
	int32_t w, h;
	layout->get_pixel_size(w, h);
	return COMBINE_INT32(w, h);
}

void DrawString(char* text, int32_t textlen, char* typeface, int32_t typefacelen, double pointsize, int32_t styles, int32_t alignment, int32_t wrapmode, int32_t wrapwidth, double r, double g, double b, double a, int32_t w, int32_t h, unsigned char* buffer)
{
	Pango::init();
	
	auto surface = Cairo::ImageSurface::create(Cairo::FORMAT_ARGB32, w, h);
	auto context = Cairo::Context::create(surface);
	context->set_source_rgba(0, 0, 0, 0);
	context->paint();
	context->set_source_rgba(r, g, b, a);
	auto layout = CreateLayout(std::string(text, textlen), std::string(typeface, typefacelen), pointsize, styles, alignment, wrapmode, wrapwidth, context);
	layout->show_in_cairo_context(context);
	
	std::memcpy(buffer, surface->get_data(), w * h * 4);
}


