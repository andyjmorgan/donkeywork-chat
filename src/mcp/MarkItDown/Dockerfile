# Use an official Python runtime as the base image
FROM python:3.10-slim

# Set working directory
WORKDIR /workdir

# Install the markitdown-mcp package
RUN pip install markitdown-mcp

# Expose the port that the SSE server will run on
EXPOSE 3001

# Command to run when the container starts
# Set to use SSE mode by default
ENTRYPOINT ["markitdown-mcp"]

# Default command parameters for SSE mode
CMD ["--sse", "--host", "0.0.0.0", "--port", "3001"]