package com.imageprocessor.core;

/**
 * Enum for available image filters
 */
public enum FilterType {
    ORIGINAL("Original", "No modification"),
    GRAYSCALE("Grayscale", "Black & white conversion"),
    INVERT("Invert", "Negative color effect"),
    BRIGHTNESS("Brightness", "Increase luminosity"),
    CONTRAST("Contrast", "Enhance light/dark difference"),
    SEPIA("Sepia", "Vintage warm tone");

    private final String displayName;
    private final String description;

    FilterType(String displayName, String description) {
        this.displayName = displayName;
        this.description = description;
    }

    public String getDisplayName() {
        return displayName;
    }

    public String getDescription() {
        return description;
    }

    public static FilterType getNext(FilterType current) {
        int nextIndex = (current.ordinal() + 1) % values().length;
        return values()[nextIndex];
    }
}
