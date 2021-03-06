# GithubReleaseHook
Github Release Hook is a tool to handle Github release event and run script.

更多细节请见中文博客: [blog.hlyue.com](https://blog.hlyue.com/2016/08/26/GithubReleaseHookDevNotes/)

## Usage

Currently I only produce docker.

Windows & Linux are primary supported platform.

You can write your own project's Dockerfile and repo.yml, your Dockerfile should `FROM richard1122/GithubReleaseHook`.

## Usage

### Github Webhook

Add webhook to your docker exposed port. Select `Let me select individual events.` and only check `Release`. Other events will be droped silently.

For security, you **should** generate unique secret key and keep it secret.

### Dockerfile

A very simple Dockerfile, copy `repo.yml` (will discuss later) into `/usr/src/app/` (which is GithubReleaseHook source dir).

```Dockerfile
FROM richard1122/githubreleasehook
RUN mkdir /blog
VOLUME /blog
COPY repo.yml /usr/src/app/
```

### Repo.yml

The following is a sample `repo.yml` for my blog.

Add one or more filenames in file directive. GithubReleaseHook will automaticly download it. Then reference each file in script directive like `$f0` for the first file.

Secret is an important key to ensure event is sent by Github. Please keep it **secret**.

```YAML
repo: richard1122/blog.hlyue.com
file:
    - release.tar.gz
script:
    - tar xavf $f0
secret: sec
workingDir: /blog
```

### Nginx

Your static file server might be Nginx running outside docker. Add Docker Volumn to `/blog` of your Docker.