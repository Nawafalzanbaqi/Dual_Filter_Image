package com.imageprocessor.core;

import javafx.scene.image.Image;
import javafx.scene.image.PixelReader;
import javafx.scene.image.PixelWriter;
import javafx.scene.image.WritableImage;
import javafx.scene.paint.Color;

/**
 * Image processing engine with various filters
 */
public class ImageProcessor {

    /**
     * Apply filter to image
     */
    public static Image applyFilter(Image sourceImage, FilterType filterType) {
        if (sourceImage == null || filterType == FilterType.ORIGINAL) {
            return sourceImage;
        }

        int width = (int) sourceImage.getWidth();
        int height = (int) sourceImage.getHeight();
        WritableImage resultImage = new WritableImage(width, height);

        PixelReader reader = sourceImage.getPixelReader();
        PixelWriter writer = resultImage.getPixelWriter();

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                Color originalColor = reader.getColor(x, y);
                Color processedColor = processPixel(originalColor, filterType);
                writer.setColor(x, y, processedColor);
            }
        }

        return resultImage;
    }

    /**
     * Process individual pixel based on filter type
     */
    private static Color processPixel(Color color, FilterType filterType) {
        return switch (filterType) {
            case GRAYSCALE -> applyGrayscale(color);
            case INVERT -> applyInvert(color);
            case BRIGHTNESS -> applyBrightness(color);
            case CONTRAST -> applyContrast(color);
            case SEPIA -> applySepia(color);
            default -> color;
        };
    }

    /**
     * Grayscale filter - Convert to black and white
     */
    private static Color applyGrayscale(Color color) {
        double gray = color.getRed() * 0.299 + 
                     color.getGreen() * 0.587 + 
                     color.getBlue() * 0.114;
        return new Color(gray, gray, gray, color.getOpacity());
    }

    /**
     * Invert filter - Reverse all colors
     */
    private static Color applyInvert(Color color) {
        return new Color(
            1.0 - color.getRed(),
            1.0 - color.getGreen(),
            1.0 - color.getBlue(),
            color.getOpacity()
        );
    }

    /**
     * Brightness filter - Increase luminosity
     */
    private static Color applyBrightness(Color color) {
        double factor = 1.2; // 20% increase
        return new Color(
            Math.min(color.getRed() * factor, 1.0),
            Math.min(color.getGreen() * factor, 1.0),
            Math.min(color.getBlue() * factor, 1.0),
            color.getOpacity()
        );
    }

    /**
     * Contrast filter - Enhance light/dark difference
     */
    private static Color applyContrast(Color color) {
        double factor = 1.5; // 50% increase
        double mid = 0.5;
        
        double r = mid + (color.getRed() - mid) * factor;
        double g = mid + (color.getGreen() - mid) * factor;
        double b = mid + (color.getBlue() - mid) * factor;
        
        return new Color(
            Math.min(Math.max(r, 0.0), 1.0),
            Math.min(Math.max(g, 0.0), 1.0),
            Math.min(Math.max(b, 0.0), 1.0),
            color.getOpacity()
        );
    }

    /**
     * Sepia filter - Vintage warm tone
     */
    private static Color applySepia(Color color) {
        double r = color.getRed() * 0.393 + color.getGreen() * 0.769 + color.getBlue() * 0.189;
        double g = color.getRed() * 0.349 + color.getGreen() * 0.686 + color.getBlue() * 0.168;
        double b = color.getRed() * 0.272 + color.getGreen() * 0.534 + color.getBlue() * 0.131;
        
        return new Color(
            Math.min(r, 1.0),
            Math.min(g, 1.0),
            Math.min(b, 1.0),
            color.getOpacity()
        );
    }

    /**
     * Create a copy of the image
     */
    public static Image copyImage(Image source) {
        if (source == null) return null;
        
        int width = (int) source.getWidth();
        int height = (int) source.getHeight();
        WritableImage copy = new WritableImage(width, height);
        
        PixelReader reader = source.getPixelReader();
        PixelWriter writer = copy.getPixelWriter();
        
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                writer.setColor(x, y, reader.getColor(x, y));
            }
        }
        
        return copy;
    }
}
