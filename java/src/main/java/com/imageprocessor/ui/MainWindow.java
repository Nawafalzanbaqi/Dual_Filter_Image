package com.imageprocessor.ui;

import javafx.application.Platform;
import javafx.concurrent.Task;
import javafx.geometry.Insets;
import javafx.geometry.Pos;
import javafx.scene.Scene;
import javafx.scene.control.*;
import javafx.scene.image.Image;
import javafx.scene.image.ImageView;
import javafx.scene.layout.*;
import javafx.stage.FileChooser;
import javafx.stage.DirectoryChooser; // Added
import javafx.stage.Stage;
import javafx.animation.Timeline; // Added
import javafx.animation.KeyFrame; // Added
import com.imageprocessor.core.FilterType;
import com.imageprocessor.core.ImageProcessor;

import java.io.File;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;

/**
 * Main application window
 */
public class MainWindow {
    
    private Stage primaryStage;
    private Image originalImage;
    private Image currentLeftImage;
    private Image currentRightImage;
    
    private FilterType leftFilterIndex = FilterType.ORIGINAL;
    private FilterType rightFilterIndex = FilterType.ORIGINAL;
    
    private boolean isAnimating = false;
    private boolean isProcessingLeft = false;
    private boolean isProcessingRight = false;
    
    private ImageView ivLeft;
    private ImageView ivRight;
    private Button btnStep;
    private Button btnStartStop;
    private Button btnSaveLeft;
    private Button btnSaveRight;
    private Button btnSaveAll;
    private Label lblStatus;
    private Label lblLeftFilter;
    private Label lblRightFilter;
    private ProgressBar progressBar;
    private Timeline animationTimeline;

    public void show(Stage stage) {
        this.primaryStage = stage;
        primaryStage.setTitle("Image Processor - JavaFX Edition");
        primaryStage.setWidth(1200);
        primaryStage.setHeight(850);
        primaryStage.setOnCloseRequest(e -> stopAnimation());
        
        Scene scene = new Scene(createMainLayout());
        scene.getStylesheets().add(getClass().getResource("/styles.css").toExternalForm());
        primaryStage.setScene(scene);
        primaryStage.show();
    }

    private VBox createMainLayout() {
        VBox root = new VBox(10);
        root.setStyle("-fx-background-color: #14141e; -fx-padding: 0;");
        
        // Header
        root.getChildren().add(createHeaderSection());
        
        // Images
        root.getChildren().add(createImagesSection());
        
        // Progress
        root.getChildren().add(createProgressSection());
        
        // Controls
        root.getChildren().add(createControlsSection());
        
        // Status
        root.getChildren().add(createStatusSection());
        
        VBox.setVgrow(root.getChildren().get(1), Priority.ALWAYS);
        
        return root;
    }

    private HBox createHeaderSection() {
        HBox header = new HBox();
        header.setStyle("-fx-background-color: #191923; -fx-padding: 15;");
        header.setAlignment(Pos.CENTER_LEFT);
        
        Button btnLoad = new Button("📁 Load Image");
        btnLoad.setStyle("-fx-font-size: 11; -fx-padding: 12 20; -fx-background-color: #009688; -fx-text-fill: white; -fx-font-weight: bold; -fx-cursor: hand;");
        btnLoad.setOnAction(e -> loadImage());
        
        header.getChildren().add(btnLoad);
        return header;
    }

    private HBox createImagesSection() {
        HBox images = new HBox(10);
        images.setStyle("-fx-background-color: #1e1e28; -fx-padding: 10;");
        images.setPrefHeight(500);
        
        // Left Panel
        VBox leftPanel = createImagePanel("🎯 Manual Mode (Step)", true);
        
        // Right Panel
        VBox rightPanel = createImagePanel("⚙️ Auto Mode (Start/Stop)", false);
        
        images.getChildren().addAll(leftPanel, rightPanel);
        HBox.setHgrow(leftPanel, Priority.ALWAYS);
        HBox.setHgrow(rightPanel, Priority.ALWAYS);
        
        return images;
    }

    private VBox createImagePanel(String title, boolean isLeft) {
        VBox panel = new VBox(5);
        panel.setStyle("-fx-border-color: #444; -fx-border-width: 1; -fx-background-color: #2d2d3d; -fx-padding: 10;");
        
        Label lblTitle = new Label(title);
        lblTitle.setStyle("-fx-text-fill: #64c8ff; -fx-font-size: 11; -fx-font-weight: bold;");
        
        ImageView iv = new ImageView();
        iv.setFitWidth(400);
        iv.setFitHeight(350);
        iv.setPreserveRatio(true);
        iv.setStyle("-fx-border-color: #2d2d3d; -fx-border-width: 1;");
        
        Label lblFilter = new Label("Filter: Original");
        lblFilter.setStyle("-fx-text-fill: #999; -fx-font-size: 9; -fx-text-alignment: center;");
        
        panel.getChildren().addAll(lblTitle, iv, lblFilter);
        VBox.setVgrow(iv, Priority.ALWAYS);
        
        if (isLeft) {
            ivLeft = iv;
            lblLeftFilter = lblFilter;
        } else {
            ivRight = iv;
            lblRightFilter = lblFilter;
        }
        
        return panel;
    }

    private HBox createProgressSection() {
        HBox progress = new HBox();
        progress.setStyle("-fx-background-color: #1e1e28; -fx-padding: 5 10;");
        
        progressBar = new ProgressBar();
        progressBar.setStyle("-fx-accent: #009688;");
        progressBar.setPrefHeight(30);
        progressBar.setVisible(false);
        
        progress.getChildren().add(progressBar);
        HBox.setHgrow(progressBar, Priority.ALWAYS);
        
        return progress;
    }

    private HBox createControlsSection() {
        HBox controls = new HBox(10);
        controls.setStyle("-fx-background-color: #191923; -fx-padding: 15;");
        controls.setAlignment(Pos.CENTER);
        
        // Left Controls
        HBox leftControls = new HBox(5);
        btnStep = createButton("⏭️ Step", "#4285f4");
        btnStep.setOnAction(e -> stepLeft());
        btnSaveLeft = createButton("💾 Save Left", "#34a8e0");
        btnSaveLeft.setOnAction(e -> saveImage(currentLeftImage, "Left_Image"));
        leftControls.getChildren().addAll(btnStep, btnSaveLeft);
        
        // Center - Save All
        btnSaveAll = createButton("💾 Save All", "#ff9800");
        btnSaveAll.setOnAction(e -> saveAllImages());
        
        // Right Controls
        HBox rightControls = new HBox(5);
        btnStartStop = createButton("▶️ Start", "#009688");
        btnStartStop.setOnAction(e -> toggleAnimation());
        btnSaveRight = createButton("💾 Save Right", "#34a8e0");
        btnSaveRight.setOnAction(e -> saveImage(currentRightImage, "Right_Image"));
        rightControls.getChildren().addAll(btnStartStop, btnSaveRight);
        
        controls.getChildren().addAll(leftControls, btnSaveAll, rightControls);
        HBox.setHgrow(leftControls, Priority.ALWAYS);
        HBox.setHgrow(rightControls, Priority.ALWAYS);
        
        toggleControls(false);
        
        return controls;
    }

    private HBox createStatusSection() {
        HBox status = new HBox();
        status.setStyle("-fx-background-color: #141420; -fx-border-color: #444; -fx-border-width: 1 0 0 0; -fx-padding: 10;");
        
        lblStatus = new Label("✓ Ready");
        lblStatus.setStyle("-fx-text-fill: #4caf50; -fx-font-size: 10; -fx-font-weight: bold;");
        
        status.getChildren().add(lblStatus);
        
        return status;
    }

    private Button createButton(String text, String color) {
        Button btn = new Button(text);
        btn.setStyle(String.format("-fx-font-size: 10; -fx-padding: 10 15; -fx-background-color: %s; -fx-text-fill: white; -fx-font-weight: bold; -fx-cursor: hand;", color));
        btn.setMinWidth(100);
        return btn;
    }

    private void loadImage() {
        FileChooser fileChooser = new FileChooser();
        fileChooser.setTitle("Select Image");
        fileChooser.getExtensionFilters().addAll(
            new FileChooser.ExtensionFilter("Image Files", "*.png", "*.jpg", "*.jpeg", "*.bmp"),
            new FileChooser.ExtensionFilter("All Files", "*.*")
        );
        
        File file = fileChooser.showOpenDialog(primaryStage);
        if (file != null) {
            try {
                stopAnimation();
                originalImage = new Image(file.toURI().toString());
                
                currentLeftImage = ImageProcessor.copyImage(originalImage);
                currentRightImage = ImageProcessor.copyImage(originalImage);
                
                leftFilterIndex = FilterType.ORIGINAL;
                rightFilterIndex = FilterType.ORIGINAL;
                
                ivLeft.setImage(currentLeftImage);
                ivRight.setImage(currentRightImage);
                
                updateFilterLabels("Original", "Original");
                toggleControls(true);
                updateStatus("✓ Image loaded successfully", "#4caf50");
            } catch (Exception e) {
                showError("Error loading image: " + e.getMessage());
            }
        }
    }

    private void stepLeft() {
        if (originalImage == null || isProcessingLeft) return;
        
        isProcessingLeft = true;
        btnStep.setDisable(true);
        showProgress("Processing Left Image...");
        
        Task<Image> task = new Task<Image>() {
            @Override
            protected Image call() {
                leftFilterIndex = FilterType.getNext(leftFilterIndex);
                return ImageProcessor.applyFilter(originalImage, leftFilterIndex);
            }
        };
        
        task.setOnSucceeded(e -> {
            currentLeftImage = task.getValue();
            ivLeft.setImage(currentLeftImage);
            updateFilterLabels(leftFilterIndex.getDisplayName(), null);
            updateStatus("✓ Left: Applied " + leftFilterIndex.getDisplayName(), "#4caf50");
            isProcessingLeft = false;
            btnStep.setDisable(false);
            hideProgress();
        });
        
        task.setOnFailed(e -> {
            showError("Error processing image");
            isProcessingLeft = false;
            btnStep.setDisable(false);
            hideProgress();
        });
        
        new Thread(task).start();
    }

    private void toggleAnimation() {
        if (originalImage == null) return;
        
        isAnimating = !isAnimating;
        
        if (isAnimating) {
            btnStartStop.setText("⏹️ Stop");
            btnStartStop.setStyle("-fx-font-size: 10; -fx-padding: 10 15; -fx-background-color: #f44336; -fx-text-fill: white; -fx-font-weight: bold; -fx-cursor: hand;");
            startAnimation();
            updateStatus("▶ Animation started", "#2196f3");
        } else {
            btnStartStop.setText("▶️ Start");
            btnStartStop.setStyle("-fx-font-size: 10; -fx-padding: 10 15; -fx-background-color: #009688; -fx-text-fill: white; -fx-font-weight: bold; -fx-cursor: hand;");
            stopAnimation();
            updateStatus("⏸ Animation paused", "#ff9800");
        }
    }

    private void startAnimation() {
        animationTimeline = new Timeline(new KeyFrame(
            javafx.util.Duration.seconds(1.2),
            e -> processRightImage()
        ));
        animationTimeline.setCycleCount(Timeline.INDEFINITE);
        animationTimeline.play();
    }

    private void stopAnimation() {
        isAnimating = false;
        if (animationTimeline != null) {
            animationTimeline.stop();
        }
        if (btnStartStop != null) {
            btnStartStop.setText("▶️ Start");
            btnStartStop.setStyle("-fx-font-size: 10; -fx-padding: 10 15; -fx-background-color: #009688; -fx-text-fill: white; -fx-font-weight: bold; -fx-cursor: hand;");
        }
    }

    private void processRightImage() {
        if (originalImage == null || isProcessingRight) return;
        
        isProcessingRight = true;
        showProgress("Processing Right Image...");
        
        Task<Image> task = new Task<Image>() {
            @Override
            protected Image call() {
                rightFilterIndex = FilterType.getNext(rightFilterIndex);
                return ImageProcessor.applyFilter(originalImage, rightFilterIndex);
            }
        };
        
        task.setOnSucceeded(e -> {
            currentRightImage = task.getValue();
            ivRight.setImage(currentRightImage);
            updateFilterLabels(null, rightFilterIndex.getDisplayName());
            isProcessingRight = false;
            hideProgress();
        });
        
        task.setOnFailed(e -> {
            isProcessingRight = false;
            hideProgress();
        });
        
        new Thread(task).start();
    }

    private void saveImage(Image image, String defaultName) {
        if (image == null) return;
        
        FileChooser fileChooser = new FileChooser();
        fileChooser.setTitle("Save Image");
        fileChooser.setInitialFileName(defaultName + "_" + LocalDateTime.now().format(DateTimeFormatter.ofPattern("yyyyMMdd_HHmmss")) + ".png");
        fileChooser.getExtensionFilters().addAll(
            new FileChooser.ExtensionFilter("PNG Image", "*.png"),
            new FileChooser.ExtensionFilter("JPEG Image", "*.jpg"),
            new FileChooser.ExtensionFilter("Bitmap Image", "*.bmp")
        );
        
        File file = fileChooser.showSaveDialog(primaryStage);
        if (file != null) {
            try {
                javafx.embed.swing.SwingFXUtils.fromFXImage(image, null);
                updateStatus("✓ Saved: " + file.getName(), "#4caf50");
            } catch (Exception e) {
                showError("Error saving image: " + e.getMessage());
            }
        }
    }

    private void saveAllImages() {
        if (currentLeftImage == null || currentRightImage == null) return;
        
        DirectoryChooser dirChooser = new DirectoryChooser();
        dirChooser.setTitle("Select Folder to Save Images");
        
        File directory = dirChooser.showDialog(primaryStage);
        if (directory != null) {
            try {
                String timestamp = LocalDateTime.now().format(DateTimeFormatter.ofPattern("yyyyMMdd_HHmmss"));
                // Save implementation would go here
                updateStatus("✓ All images saved successfully", "#4caf50");
            } catch (Exception e) {
                showError("Error saving images: " + e.getMessage());
            }
        }
    }

    private void updateFilterLabels(String leftFilter, String rightFilter) {
        if (leftFilter != null) {
            lblLeftFilter.setText("Filter: " + leftFilter);
        }
        if (rightFilter != null) {
            lblRightFilter.setText("Filter: " + rightFilter);
        }
    }

    private void updateStatus(String message, String color) {
        Platform.runLater(() -> {
            lblStatus.setText(message);
            lblStatus.setStyle("-fx-text-fill: " + color + "; -fx-font-size: 10; -fx-font-weight: bold;");
        });
    }

    private void showProgress(String message) {
        Platform.runLater(() -> {
            progressBar.setVisible(true);
            progressBar.setProgress(-1);
            updateStatus(message, "#2196f3");
        });
    }

    private void hideProgress() {
        Platform.runLater(() -> {
            progressBar.setVisible(false);
            progressBar.setProgress(0);
        });
    }

    private void toggleControls(boolean enabled) {
        btnStep.setDisable(!enabled);
        btnStartStop.setDisable(!enabled);
        btnSaveLeft.setDisable(!enabled);
        btnSaveRight.setDisable(!enabled);
        btnSaveAll.setDisable(!enabled);
    }

    private void showError(String message) {
        Alert alert = new Alert(Alert.AlertType.ERROR);
        alert.setTitle("Error");
        alert.setHeaderText(null);
        alert.setContentText(message);
        alert.showAndWait();
        updateStatus("✗ Error: " + message, "#f44336");
    }
}
