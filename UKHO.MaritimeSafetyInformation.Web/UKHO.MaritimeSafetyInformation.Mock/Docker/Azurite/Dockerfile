# Use the official Azurite image from Docker Hub
FROM mcr.microsoft.com/azure-storage/azurite

# Expose the default ports for Azurite
EXPOSE 10000 10001 10002

# Command to start Azurite
CMD ["azurite", "-l", "/data", "--blobHost", "0.0.0.0", "--queueHost", "0.0.0.0", "--tableHost", "0.0.0.0"]
