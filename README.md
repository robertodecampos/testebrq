# testebrq

**Criação do Banco de Dados**

    CREATE DATABASE testebrq;
    
    USE testebrq;
    
    CREATE TABLE `usuario` (
      `id` INT(11) NOT NULL AUTO_INCREMENT,
      `login` VARCHAR(50) NOT NULL,
      `senha` VARCHAR(50) NOT NULL,
      PRIMARY KEY (`id`)
    );
    
    CREATE TABLE `log_tentativa_login` (
      `id` INT(11) NOT NULL AUTO_INCREMENT,
      `login` VARCHAR(50) DEFAULT NULL,
      `dataTentativa` DATETIME DEFAULT NULL,
      `loginEfetuado` TINYINT(1) DEFAULT '0',
      PRIMARY KEY (`id`)
    );
    
    
Após a criação do banco de dados, é necessário cadastrar um usuário para que possa realizar o teste:
    
    INSERT INTO `usuario` (login, senha) VALUES ('adm', '123');
        
Com o usuário no banco, podemos realizar uma requisição para recuperar o JWT:

`caminho:porta/api/token/gerar?login=adm&senha=123`

Agora possuímos o JWT, podemos listar todas as tentativas de login efetuadas para o usuário autenticado.

    GET /api/tentativalogin/listar HTTP/1.1
    Host: caminho:porta
    Authorization: Bearer token_gerado
