# Etapa 1: Construção do Angular
FROM node:18 AS build

WORKDIR /app

# Copiar apenas os arquivos essenciais para a instalação das dependências
COPY angular-task-manager/package*.json ./

RUN npm install

# Copiar o restante do projeto
COPY angular-task-manager/ ./

# Construir a aplicação Angular
RUN npm run build

# Etapa 2: Servindo com Nginx
FROM nginx:alpine

# Copiar os arquivos do build Angular para o diretório correto do Nginx
COPY --from=build /app/dist/angular-task-manager/browser /usr/share/nginx/html

# Expor a porta 80
EXPOSE 80

# Comando padrão do Nginx
CMD ["nginx", "-g", "daemon off;"]
