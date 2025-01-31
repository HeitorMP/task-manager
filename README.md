# Task Management Application

This is a task management application built with Angular on the frontend and a backend that can be set up with Docker. Below are the instructions to run the application locally using Docker and Docker Compose.

## Dependencies

To run this application, you need to have the following dependencies installed:

- **Docker**: [Link to Docker installation](https://www.docker.com/get-started)
- **Docker Compose**: Docker Compose is typically installed with Docker, but you can check installation instructions [here](https://docs.docker.com/compose/install/).

## How to Run the Application

Follow the steps below to run the application on your local machine:

### 1. Clone the Repository

First, clone the repository to your local machine:

```bash
git clone git@github.com:HeitorMP/task-manager.git
cd task-manager
```

### 2. Start the Containers with Docker Compose

Make sure Docker and Docker Compose are correctly installed and running on your machine.

In the directory of your project, where the `docker-compose.yml` file is located, run the following command to start all the containers:

```bash
docker-compose up --build
```

This command will:

- Download any required images (if not already locally available)
- Build the service images
- Start the containers defined in `docker-compose.yml`

### 3. Accessing the Application

Once the containers are up and running, you can access the application through your browser at:

```
http://localhost
```

Here you will be able to view the Angular frontend and interact with the backend, which will be running at the same address.

### 4. Stopping the Application

When you are done using the application, you can stop the containers with the following command:

```bash
docker-compose down
```

This will stop and remove the containers, but will keep the images. If you want to remove the images as well, use the command `docker-compose down --rmi all`.

## Project Structure

Here is an overview of the project structure:

```
/
|-- docker-compose.yml       # Docker Compose configuration file
|-- Dockerfile               # Dockerfile for building the backend or frontend
|-- frontend/                # Frontend source code (Angular)
|-- backend/                 # Backend source code
|-- .env                     # Environment variables (if needed)
```



